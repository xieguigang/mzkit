#!REnv

# imports mzkit library modules
imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

# includes external helper script
imports "plot_ionRaws.R";

# config of the standard curve data files
let wiff     as string = ?"--Cal"          || stop("No standard curve data provides!");
let sample   as string = ?"--data"         || stop("No sample data files provided!");
let MRM.info as string = ?"--MRM"          || stop("Missing MRM information table file!");
# use external MSL data file if there is no 
# ion pair data in the MRM table file. 
let ions     as string = ?"--ions";         
let dir      as string = ?"--export"       || `${wiff :> trim(" ")}-result/`;
# The regexp pattern of the file name for match
# the reference point data.
let patternOf.ref      = ?"--patternOfRef" || '[-]?LM[-]?\d+';
let patternOf.QC       = ?"--patternOfQC"  || "QC[-]?\d+";
 
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
let integrator  as string  = ?"--integrator" || "NetPeakSum";
let isWorkCurve as boolean = ?"--workMode";
let rt_winSize  as double  = as.numeric(?"--rt.winsize" || "3"); 
let tolerance   as string  = ?"--mz.diff"    || "ppm:30";

# Max number of points for removes in 
# linear modelling
#
# + negative value for auto detects: n.points / 2 - 1
# + ZERO for no points is removed
# + positive value for specific a number for the deletion.
let maxNumOfPoint.delets = ?"--max.deletes"       || -1;

let angle.threshold      = ?"--angle.threshold"   || 5;
let baseline.quantile    = ?"--baseline.quantile" || 0.65;

if (isWorkCurve) {
	print("Linear Modelling will running in work curve mode!");
}

print("View parameter configurations:");
print("RT window size:");
print(rt_winSize);
print("m/z tolerance for find MRM ion:");
print(tolerance);
print("Integrator that we used for calculate the Peak Area:");
print(integrator);
print("Max number of points that allowes removes automatically in the process of linear modelling:");

if (maxNumOfPoint.delets < 0) {
	print("It's depends on the number of reference sample");
} else {
	if (maxNumOfPoint.delets == 0) {
		print("Is not allowed for removes any points!");
	} else {
		print(`Removes less than ${maxNumOfPoint.delets} bad reference points.`);
	}
} 

print(`MRM ion peak is populated from raw data with angle threshold ${angle.threshold}.`);
print(`All of the data ticks that its intensity value less than ${baseline.quantile} quantile level will be treated as background noise`);

let reference = NULL;
let is        = NULL;

# read MRM, standard curve and IS information from the given file
if (file.exists(ions)) {
	[reference, is] = MRM.info :> [		
		read.reference("coordinates"), 
		read.IS("IS")
	];	
	
	print("Use external msl data as ion pairs.");
	
	ions = ions 
	# the time unit is minute by default
	# required convert to second by 
	# specific that the time unit is Minute
	# at here
	:> read.msl(unit = "Minute") 
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

if (length(is) == 0) {
	print("No internal standards...");
} else {
	print(is);
}

print("Ion pairs for each required metabolites:");
print(ions);
print("Previews of the isomerism ion pairs:");
print(ions :> isomerism.ion_pairs);

print(`The reference data raw files will be matches by name pattern: [${patternOf.ref}]`);

wiff <- list(samples = sample, reference = wiff) 
# :> wiff.rawfiles("[-]?LM[-]?\d+") 
:> wiff.rawfiles(patternOf.ref) 
:> as.object;

print("Reference standards:");
print(basename(wiff$standards));
print("Sample data files:");
print(basename(wiff$samples));

let blanks <- NULL;
let QC_samples = basename(wiff$samples) like regexp(patternOf.QC);

if (sum(QC_samples) > 0) {
	print(`Find ${sum(QC_samples)} in raw data:`);
	print(basename(wiff$samples[QC_samples]));
}

if (wiff$hasBlankControls) {
	print(`There are ${length(wiff$blanks)} blank controls in wiff raw data!`);
	print(wiff$blanks);

	blanks = wiff$blanks :> wiff.scans(
		ions             = ions, 
		peakAreaMethod   = integrator, 
		TPAFactors       = NULL,
		tolerance        = tolerance,
		timeWindowSize   = rt_winSize,
		removesWiffName  = TRUE,
		angleThreshold   = angle.threshold,
        baselineQuantile = baseline.quantile
	);
} else {
	print("Target reference data have no blank controls.");
}

#' Create linear models
#'
#' @param wiff_standards A file path collection of ``*.mzML`` files, which should be the reference points.
#' @param subdir A directory name for save the result table
#'
let linears.standard_curve as function(wiff_standards, subdir) {
	let rt.shifts = wiff_standards :> MRM.rt_alignments(ions, args = MRM.arguments(
		tolerance        = tolerance,
		timeWindowSize   = rt_winSize,
		angleThreshold   = angle.threshold,
		baselineQuantile = baseline.quantile,
		peakAreaMethod   = integrator,
		TPAFactors       = NULL
	));
	
	print("Previews of the rt shifts summary in your sample reference points:");
	
	rt.shifts
	:> as.data.frame
	:> print
	;
	
	rt.shifts
	:> as.data.frame
	:> write.csv(file = `${dir}/${subdir}/rt_shifts.csv`)
	;
	
	# Get raw scan data for given ions
	let CAL <- wiff_standards 
	# list.files(wiff, pattern = "*.mzML")
	:> wiff.scans(
 		ions             = ions, 
 		peakAreaMethod   = integrator, 
	 	TPAFactors       = NULL,
		tolerance        = tolerance,
		timeWindowSize   = rt_winSize,
		removesWiffName  = TRUE,
		angleThreshold   = angle.threshold,
		baselineQuantile = baseline.quantile,
		rtshifts         = NULL# rt.shifts
	);

	CAL :> write.csv(file = `${dir}/${subdir}/referencePoints(peakarea).csv`);

	let ref <- linears(
		rawScan         = CAL, 
		calibrates      = reference, 
		ISvector        = is, 
		autoWeighted    = TRUE, 
		blankControls   = blanks, 
		maxDeletions    = maxNumOfPoint.delets, 
		isWorkCurveMode = isWorkCurve
	);

	#' print model summary and then do standard curve plot
	#'
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
		let fileName <- basename(mzML);
		let peaks    <- MRM.peaks(
			mzML             = mzML, 
			ions             = ions, 
			peakAreaMethod   = integrator, 
			TPAFactors       = NULL, 
			tolerance        = tolerance, 
			timeWindowSize   = rt_winSize,
			angleThreshold   = angle.threshold,
			baselineQuantile = baseline.quantile
		);
		
		# save peaktable for given rawfile
		write.csv(peaks, file = `${dir}/${subdir}/peaktables/${fileName}.csv`);
	}	
	
	return ref;
}

let doLinears as function(wiff_standards, subdir = "") {
	let scans    = [];
	let ref      = linears.standard_curve(wiff_standards, subdir);
	let ref_raws = ions 
	# get ion chromatograms raw data for 
	# TIC data plots
	:> getIonsSampleRaw(wiff_standards, tolerance) 
	:> lapply(ion => ion$chromatograms)
	;
	
	# calculate standards points as well for quality controls
	# and result data verification
	let sample.files = wiff$samples << wiff_standards; 

	# Write raw scan data of the user sample data
	sample.files
	# list.files(wiff, pattern = "*.mzML")
	:> wiff.scans(
		ions             = ions, 
		peakAreaMethod   = integrator, 
		TPAFactors       = NULL, 
		tolerance        = tolerance, 
		removesWiffName  = TRUE, 
		timeWindowSize   = rt_winSize,
		angleThreshold   = angle.threshold,
		baselineQuantile = baseline.quantile
	) 
	:> write.csv(file = `${dir}/${subdir}\samples.csv`);

	# create ion quantify result for each metabolites
	# that defined in ion pairs data
	for(sample.mzML in sample.files) {
		let peakfile as string = `${dir}/${subdir}/samples_peaktable/${basename(sample.mzML)}.csv`;
		let result = ref :> sample.quantify(
			sample.mzML, ions, 
			peakAreaMethod   = integrator, 
			tolerance        = tolerance, 
			timeWindowSize   = rt_winSize, 
			TPAFactors       = NULL,
			angleThreshold   = angle.threshold,
		    baselineQuantile = baseline.quantile
		);
		
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
	
	print("Creating linear model report....");
	
	# save linear regression html report
	ref
	:> mrm.dataset(scans, ionsRaw = ref_raws) 
	:> html 
	:> writeLines(con = `${dir}/${subdir}/index.html`)
	;
		
	if (sum(QC_samples) > 0) {
		print("Creating QC report....");
	
		ref
		:> mrm.dataset(scans, QC_dataset = patternOf.QC) 
		:> html 
		:> writeLines(con = `${dir}/${subdir}/QC.html`)
		;	
	} else {
		print("QC report will not created due to the reason of no QC samples...");
	}
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