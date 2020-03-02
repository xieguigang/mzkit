#!REnv

# imports mzkit library modules
imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

# config of the standard curve data files
let wiff as string     = ?"--Cal"          || stop("No standard curve data provides!");
let sample as string   = ?"--data"         || stop("No sample data files provided!");
let MRM.info as string = ?"--MRM"          || stop("Missing MRM information table file!");
let ions as string     = ?"--ions";         
let dir as string      = ?"--export"       || `${wiff :> trim(" ")}-result/`;
let patternOf.ref      = ?"--patternOfRef" || '[-]?LM[-]?\d+';

# let Methods as integer = {
      # NetPeakSum = 0;
      # Integrator = 1;
      # SumAll = 2;
      # MaxPeakHeight = 3;
# }

# peak area intergration calculation method
# these api functions that required of the integrator parameter
#
# 1. sample.quantify
# 2. wiff.scans
# 3. MRM.peaks
# 4. extract.peakROI
#
let integrator as string   = ?"--integrator" || "NetPeakSum";
let isWorkCurve as boolean = ?"--workMode";

if (isWorkCurve) {
	print("Linear Modelling will running in work curve mode!");
}

let reference;
let is;

# read MRM, standard curve and IS information from the given file
if (file.exists(ions)) {
	[reference, is] = MRM.info :> [		
		read.reference("coordinates"), 
		read.IS("IS")
	];	
	
	print("Use external msl data as ion pairs.");
	
	ions = ions 
	:> read.msl 
	:> as.ion_pairs;
} else {
	[ions, reference, is] = MRM.info :> [
		read.ion_pairs("ion pairs"), 
		read.reference("coordinates"), 
		read.IS("IS")
	];
}

# print debug message
print("View reference standard levels data:");
print(reference);
print("Internal standards:");
print(is);
print("Ion pairs for each required metabolites:");
print(ions);

print(`The reference data raw files will be matches by name pattern: [${patternOf.ref}]`);

wiff <- list(samples = sample, reference = wiff) 
# :> wiff.rawfiles("[-]?LM[-]?\d+") 
:> wiff.rawfiles(patternOf.ref) 
:> as.object;

print("Reference standards:");
print(basename(wiff$standards));

let blanks <- NULL;

if (wiff$hasBlankControls) {
	print(`There are ${length(wiff$blanks)} blank controls in wiff raw data!`);
	print(wiff$blanks);

	blanks = wiff$blanks :> wiff.scans(
		ions           = ions, 
		peakAreaMethod = integrator, 
		TPAFactors     = NULL
	);
} else {
	print("Target reference data have no blank controls.");
}

let linears.standard_curve as function(wiff_standards, subdir) {
	# Get raw scan data for given ions
	let CAL <- wiff_standards 
	# list.files(wiff, pattern = "*.mzML")
	 :> wiff.scans(
 		ions           = ions, 
 		peakAreaMethod = integrator, 
	 	TPAFactors     = NULL
	 );

	CAL :> write.csv(file = `${dir}/${subdir}/referencePoints(peakarea).csv`);

	let ref <- linears(CAL, reference, is, autoWeighted = TRUE, blankControls = blanks, maxDeletions = -1, isWorkCurveMode = isWorkCurve);

	# print model summary and then do standard curve plot
	let printModel as function(line) {
		# get compound id name
		let id as string = line 
		:> as.object 
		:> do.call("name");
		
		# view summary result
		print(line);
		
		line
		:> standard_curve(title = `Standard Curve Of ${id}`)
		:> save.graphics(file = `${dir}/${subdir}/standard_curves/${id}.png`)
		;
		
		# save reference points
		line
		:> points(name = id)
		:> write.points(file = `${dir}/${subdir}/standard_curves/${id}.csv`)
		;
	}

	for(line in ref) {
		if (line :> as.object :> do.call("isValid")) {
			line :> printModel;
		}
	}

	# save linear models summary
	ref
	:> lines.table
	:> write.csv(file = `${dir}/${subdir}/linears.csv`)
	;

	for(mzML in wiff_standards) {
		let fileName = basename(mzML);
		let peaks = MRM.peaks(mzML, ions, peakAreaMethod = integrator, TPAFactors = NULL);
		
		# save peaktable for given rawfile
		write.csv(peaks, file = `${dir}/${subdir}/peaktables/${fileName}.csv`);
	}	
	
	return ref;
}

let doLinears as function(wiff_standards, subdir = "") {
	let scans        = [];
	let ref          = linears.standard_curve(wiff_standards, subdir);
	# calculate standards points as well for quality controls
	# and result data verification
	let sample.files = wiff$samples << wiff_standards; 

	# Write raw scan data of the user sample data
	sample.files
		# list.files(wiff, pattern = "*.mzML")
		:> wiff.scans(ions, peakAreaMethod = integrator, TPAFactors = NULL) 
		:> write.csv(file = `${dir}/${subdir}\samples.csv`);

	# create ion quantify result for each metabolites
	# that defined in ion pairs data
	for(sample.mzML in sample.files) {
		let peakfile as string = `${dir}/${subdir}/samples_peaktable/${basename(sample.mzML)}.csv`;
		let result = ref :> sample.quantify(sample.mzML, ions, integrator, NULL);
		
		print(basename(sample.mzML));
		
		result 
		:> as.object 
		:> do.call("MRMPeaks") 
		:> write.MRMpeaks(file = peakfile);
		
		scans <- scans << result;
	}

	print("Sample raw files that we scans:");
	print(length(scans));

	# save the MRM quantify result
	# base on the linear fitting
	result(scans)  :> write.csv(file = `${dir}/${subdir}\quantify.csv`);
	scans.X(scans) :> write.csv(file = `${dir}/${subdir}\rawX.csv`);
	
	# save linear regression html report
	html(mrm.dataset(ref, scans)) :> writeLines(con = `${dir}/${subdir}/index.html`);
}

if (wiff$numberOfStandardReference > 1) {
	# test for multiple standard curves
	let groups = wiff$GetLinearGroups() :> as.list;
	
	print("We get linear groups:");
	print(groups);
	
	for(linear_groupKey in names(groups)) {
		print(`Run linear profiles for '${linear_groupKey}'`);
		print(groups[[linear_groupKey]]);
	
		doLinears(groups[[linear_groupKey]], linear_groupKey);
	}	
	
} else {
	wiff$standards :> doLinears();
}

print("MRM quantify [JOB DONE!]");