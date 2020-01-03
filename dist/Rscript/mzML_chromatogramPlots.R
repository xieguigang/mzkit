imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";

let file.mzML as string = ?"--mzML" || stop("Missing the MRM mzML raw data file!");
let MRM.xlsx as string  = ?"--MRM"  || stop("No mrm information provided!");

# Get ion pair information from 
# a given excel table
let ions <- read.ion_pairs(MRM.xlsx, "ion pairs");
let raw  <- [?"--mzML"]
# Extract ion peaks from given MRM data file
# the ion data is specific by the MRM ion pairs
:> extract.ions(ionpairs = ions);

# do file data plots
let output as string = ?"--output" || `${dirname(file.mzML)}/${basename(file.mzML)}.chromatogramPlots`;

for(ion in raw :> projectAs(as.object)) {
	let save as string = `${output}/chromatogramROI/${ion$name}.png`;
	
	# draw MRM peak for each metabolite ion
	# 
	# 1. [blue] Area Integration, is the peak area integration plot
	# 2. [red] chromatography ROI, the region of interseted, MRM chromatogram peak region, from rtmin to rtmax
	# 3. [green] baseline, The chromatogram signal baseline, the lower of the baseline it is, the better of the ion signal it is
	# 4. [blank] chromatogram, The MRM ion chromatogram signal data
	#
	ion$value 
	:> MRM.chromatogramPeaks.plot(title = ion$description)
	:> save.graphics(file = save)
	;
}

# Draw all of the MRM ions onto one chromatogram plot image
[?"--mzML"]
:> chromatogram.plot(ions) 
:> save.graphics(file = `${output}/chromatogram.png`);