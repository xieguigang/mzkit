imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

let MRM       as string = ?"--MRM"     || stop("No MRM ion pair information provided!");
let mzML      as string = ?"--data"    || stop("No raw data provided! This parameter should be an exists directory.");
let export    as string = ?"--out"     || `${dirname(mzML)}/{basename(mzML)}_ions_chromatogram/`;
let tolerance as string = ?"--mz.diff" || "ppm:20";

if (lcase(file.info(MRM)$Extension) == ".xlsx") {
	# ion pair sheet table
	MRM <- read.ion_pairs(MRM, "ion pairs");
} else {
	# msl file
	MRM <- MRM 
	:> read.msl(unit = "Minute") 
	:> as.ion_pairs
	;
}

mzML <- list.files(mzML, pattern = "*.mzML");

for(ion in MRM) {
	let chromatograms = lapply(mzML, function(path) {
		as.object(extract.ions(path, ion, tolerance)[1])$chromatogram;
	});

	names(chromatograms) <- basename(mzML);
	
	chromatograms 
	:> MRM.chromatogramPeaks.plot
	:> save.graphics(file = `${export}/{as.object(ion)$accession}.png`)
	;
}