imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

# The ``--mzML`` raw data file parameter its value can be 
# the file path of a single mzML raw data file, or could
# be a directory path which the directory contains multiple
# mzML raw data files. 
let file.mzML as string  = ?"--mzML"   || stop("Missing the MRM mzML raw data file!");
# Only requires the ion pairs data in this data table file
# The table file could be a csv plain text file or xlsx 
# datasheet
let ions as string       = ?"--ions"   || stop("missing MRM ion pairs information!");
let output.dir as string = ?"--output" || `${dirname(file.mzML)}/${basename(file.mzML)}.chromatogramPlots`;
	
if ((!file.exists(ions))) {
   stop("No mrm information provided!");
}

if (lcase(file.info(ions)$Extension) == ".msl") {
	print("Use external msl file as ion pairs source.");
	
	ions = ions 
		:> read.msl 
		:> as.ion_pairs;
} else {
	# Get ion pair information from 
	# a given excel table
	ions <- read.ion_pairs(ions, "ion pairs");
}

let plot.mzML as function(file, output) {

	let raw  <- file
	  # Extract ion peaks from given MRM data file
	  # the ion data is specific by the MRM ion pairs
	  :> extract.ions(ionpairs = ions);
	  
	for(ion in raw :> projectAs(as.object)) {
		let save as string = `${output}/chromatogramROI/${ion$name}.png`;
		
		# draw MRM peak for each metabolite ion
		# 
		# 1. [blue] Area Integration, is the peak area integration plot
		# 2. [red] chromatography ROI, the region of interseted, MRM chromatogram peak region, from rtmin to rtmax
		# 3. [green] baseline, The chromatogram signal baseline, the lower of the baseline it is, the better of the ion signal it is
		# 4. [blank] chromatogram, The MRM ion chromatogram signal data
		#
		ion$chromatogram
		:> MRM.chromatogramPeaks.plot(title = ion$description)
		:> save.graphics(file = save)
		;
	}

	# Draw all of the MRM ions onto one chromatogram plot image
	file
	:> chromatogram.plot(ions, labelLayoutTicks = 3000) 
	:> save.graphics(file = `${output}/chromatogram.png`);
}

if (dir.exists(file.mzML)) {	
	# do multiple file data plots
	file.mzML <- list.files(file.mzML, pattern = "*.mzML");
	
	print("Do plot of the raw data files:");
	print(basename(file.mzML));
	
	for(filepath in file.mzML) {
		plot.mzML(filepath, `${output.dir}/${basename(filepath)}`);
		
		print(filepath);
		print("[JOB DONE!]");
	}
	
} else {
	# do single file data plots
	print("Do plot of the single raw data file:");
	print(file.mzML);
	
	plot.mzML(file.mzML, output.dir);
}