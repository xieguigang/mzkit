imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";

let file.mzML as string = ?"--mzML" || stop("Missing the MRM mzML raw data file!");

# Get ion pair information from 
# a given excel table
let ions <- read.ion_pairs(?"--mrm", "ion pairs");
let raw  <- [?"--mzML"]
# Extract ion peaks from given MRM data file
# the ion data is specific by the MRM ion pairs
:> extract.ions(ionpairs = ions);

# do file data plots
let output as string = ?"--output" || `${dirname(file.mzML)}/${basename(file.mzML)}.chromatogramPlots`;

for(ion in raw :> projectAs(as.object)) {
	let save as string = `${output}/chromatogramROI/${ion$name}.png`;
	
	ion$value 
	:> MRM.chromatogramPeaks.plot(title = ion$description)
	:> save.graphics(file = save)
	;
}

[?"--mzML"]
:> chromatogram.plot(ions) 
:> save.graphics(file = `${output}/chromatogram.png`);