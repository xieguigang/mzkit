#' title: MRM quantification
#' author: xieguigang <gg.xie@bionovogene.com>
#'
#' description: do LC-MSMS targeted metabolomics quantitative analysis
#'    base on the MRM ion pairs data. This script will take a set of 
#'    *.mzML raw data files, and then create linear models based on the
#'    linear reference raw data files, do sample quantitative evaluation
#'    and QC assertion if the QC file is exists in your sample files.

# require(mzkit);

# imports mzkit library modules
imports ["Linears", "MRMLinear", "visualPlots"] from "mzkit.quantify";
imports "assembly" from "mzkit";

# includes external helper script
imports "plot_ionRaws.R";

# config of the standard curve data files
[@info "The folder path of the reference lines. you can set the reference name pattern via '--patternOfRef' parameter for matched the raw data files in this folder."]
[@type "folder, *.mzML"]
let wiff     as string = ?"--Cal"  || stop("No standard curve data provides!");

[@info "The folder path of the sample data files."]
[@type "folder, *.mzML"]
let sample   as string = ?"--data" || stop("No sample data files provided!");

[@info "MRM ion information xlsx table file. This table file must contains the linear reference content data of each targeted metabolite for create linear reference models."]
[@type "*.xlsx"]
let MRM.info as string = ?"--MRM"  || stop("Missing MRM information table file!");
# use external MSL data file if there is no 
# ion pair data in the MRM table file. 
[@info "The *.MSL ion file for specific the MRM ion pairs data if there is no ion pair data in the MRM table."]
[@type "*.MSL"]
let ions     as string = ?"--ions";      
[@info "folder location for save quantification result output."]   
[@type "folder"]
let dir      as string = ?"--export"       || `${wiff :> trim(" ")}-result/`;
# The regexp pattern of the file name for match
# the reference point data.
[@info "the regexp expression pattern for match of the reference lines raw data file."]
[@type "regexp"]
const patternOf.ref as string   = ?"--patternOfRef" || '[-]?LM[-]?\d+';
[@info "the regexp expression pattern for match of the QC sample raw data files."]
[@type "regexp"]
const patternOf.QC as string    = ?"--patternOfQC"  || "QC[-]?\d+";
[@info "the regexp expression pattern for match of the blank sample raw data files."]
[@type "regexp"]
const patternOf.Blank as string = ?"--patternOfBLK" || "BLK(\s*\(\d+\))?";
 
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
[@info "the peak area integrator algorithm name."]
[@type "term"]
let integrator   as string  = ?"--integrator" || "NetPeakSum";
[@info "Create of the linear reference in work curve mode?"]
let isWorkCurve  as boolean = ?"--workMode";
[@info "the window size for match the RT value in MSL ion data with the RT value that detected by the peak in samples. The data unit of this parameter should be in 'second', not 'minute'."]
[@type "time window in seconds"]
let rt_winSize   as double  = as.numeric(?"--rt.winsize" || 5); 
[@info "The m/z tolerance value for match the MRM ion pair in format of mzkit tolerance syntax. Value of this mass tolerance can be da:xxx (delta mass) or ppm:xxx (ppm precision)."]
[@type "mzError"]
let tolerance    as string  = ?"--mz.diff"      || "ppm:15";
[@info "the time range of a peak, this parameter is consist with two number for speicifc the upper bound and lower bound of the peak width which is represented with RT dimension."]
[@type "doublerange"]
let peakwidth    as string  = ?"--peakwidth"    || "8,35";
[@info "the threshold value for determine that a detected peak is noise data or not. ZERO or negative value means not measure s/n cutoff."]
let sn_threshold as double  = ?"--sn_threshold" || "2";

# Max number of points for removes in 
# linear modelling
#
# + negative value for auto detects: n.points / 2 - 1
# + ZERO for no points is removed
# + positive value for specific a number for the deletion.
[@info "Max number of reference points for removes in linear modelling. The default value '-1' means auto detects."]
let maxNumOfPoint.delets as integer = ?"--max.deletes"   || -1;
[@info "The angle threshold for detect a peak via the calculation of sin(x)."]
let angle.threshold as double   = ?"--angle.threshold"   || 8;
[@info "quantile threshold value for detected baseline noise in the peak finding."]
let baseline.quantile as double = ?"--baseline.quantile" || 0.5;

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
print("peak width range(unit in second):");
print(peakwidth);
print("signal/noise ratio threshold is:");
print(sn_threshold);

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
:> wiff.rawfiles(patternOf.ref, patternOfBlank = patternOf.Blank) 
:> as.object
;

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

const args = MRM.arguments(
	tolerance        = tolerance,
	timeWindowSize   = rt_winSize,
	angleThreshold   = angle.threshold,
	baselineQuantile = baseline.quantile,
	peakAreaMethod   = integrator,
	TPAFactors       = NULL,
	peakwidth        = peakwidth,
	sn_threshold     = sn_threshold
);

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
        baselineQuantile = baseline.quantile,
		peakwidth        = peakwidth,
		sn_threshold     = sn_threshold
	);
} else {
	print("Target reference data have no blank controls.");
}

#' Create linear models
#'
#' @param wiff_standards A file path collection of ``*.mzML`` files, which should be the reference points.
#' @param subdir A directory name for save the result table
#'
const linears.standard_curve as function(wiff_standards, subdir) {
	const rt.shifts = wiff_standards :> MRM.rt_alignments(ions, args);
	
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
	const CAL <- wiff_standards 
	# list.files(wiff, pattern = "*.mzML")
	:> wiff.scan2(
 		ions             = ions,  		
		removesWiffName  = TRUE,		
		rtshifts         = NULL, # rt.shifts
		args             = args
	);
	const ref <- linears(
		rawScan         = CAL, 
		calibrates      = reference, 
		ISvector        = is, 
		autoWeighted    = TRUE, 
		blankControls   = blanks, 
		maxDeletions    = maxNumOfPoint.delets, 
		isWorkCurveMode = isWorkCurve,
		args            = args
	);

	CAL :> write.csv(file = `${dir}/${subdir}/referencePoints(peakarea).csv`);

	for(line in ref) {
		if (line :> as.object :> do.call("isValid")) {
			line :> printModel(subdir);
		}
	}

	# save linear models summary
	ref
	:> lines.table
	:> write.csv(file = `${dir}/${subdir}/linears.csv`)
	;

	for(mzML in wiff_standards) {		
		const filepath <- `${dir}/${subdir}/peaktables/${basename(mzML)}.csv`;
		const peaks    <- MRM.peak2(mzML = mzML, ions = ions, args = args);
		
		# save peaktable for given rawfile
		write.csv(peaks, file = filepath);
	}	
	
	ref;
}

#' print model summary and then do standard curve plot
#'
const printModel as function(line, subdir) {
	# get compound id name
	const id as string = line 
	:> as.object 
	:> do.call("name");
	
	# view summary result
	print(line);
	
	bitmap(file = `${dir}/${subdir}/standard_curves/${id}.png`) {
		line
		:> standard_curve(title = `Standard Curve Of ${id}`)
		;
	}
	
	# save reference points
	line
	:> points(nameRef = id)
	:> write.points(file = `${dir}/${subdir}/standard_curves/${id}.csv`)
	;
}

const doLinears as function(wiff_standards, subdir = "") {
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
	const sample.files = wiff$samples << wiff_standards; 

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
		baselineQuantile = baseline.quantile,
		peakwidth        = peakwidth,
		sn_threshold     = sn_threshold
	) 
	:> write.csv(file = `${dir}/${subdir}/samples.csv`);

	# create ion quantify result for each metabolites
	# that defined in ion pairs data
	for(sample.mzML in sample.files) {
		const peakfile as string = `${dir}/${subdir}/samples_peaktable/${basename(sample.mzML)}.csv`;
		const result = ref :> sample.quantify(
			sample.mzML, ions, 
			peakAreaMethod   = integrator, 
			tolerance        = tolerance, 
			timeWindowSize   = rt_winSize, 
			TPAFactors       = NULL,
			angleThreshold   = angle.threshold,
		    baselineQuantile = baseline.quantile,
			peakwidth        = peakwidth,
			sn_threshold     = sn_threshold
		);
		
		print(basename(sample.mzML));
		
		# QuantifyScan
		result 
		:> as.object 
		:> do.call("ionPeaks") 
		:> write.ionPeaks(file = peakfile);
		
		scans <- scans << result;
	}

	print("Sample raw files that we scans:");
	print(length(scans));

	# save the MRM quantify result
	# base on the linear fitting
	result(scans)  :> write.csv(file = `${dir}/${subdir}/quantify.csv`);
	scans.X(scans) :> write.csv(file = `${dir}/${subdir}/rawX.csv`);
	
	print("Creating linear model report....");
	
	# save linear regression html report
	ref
	:> report.dataset(scans, ionsRaw = ref_raws) 
	:> html 
	:> writeLines(con = `${dir}/${subdir}/index.html`)
	;
		
	if (sum(QC_samples) > 0) {
		print("Creating QC report....");
	
		ref
		:> report.dataset(scans, QC_dataset = patternOf.QC) 
		:> html 
		:> writeLines(con = `${dir}/${subdir}/QC.html`)
		;	
	} else {
		print("QC report will not created due to the reason of no QC samples...");
	}
}

if (wiff$numberOfStandardReference > 1) {
	# test for multiple standard curves
	const groups = wiff$GetLinearGroups() :> as.list;
	
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