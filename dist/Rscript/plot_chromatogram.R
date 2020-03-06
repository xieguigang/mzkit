imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

imports "plot_ionRaws.R";

let MRM       as string = ?"--MRM"     || stop("No MRM ion pair information provided!");
let mzML      as string = ?"--data"    || stop("No raw data provided! This parameter should be an exists directory.");
let export    as string = ?"--out"     || `${dirname(mzML)}/${basename(mzML)}_ions_chromatogram/`;
let tolerance as string = ?"--mz.diff" || "da:0.3";

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

# plot_ionRaws(MRM, list.files(mzML, pattern = "*.mzML"), tolerance, export);

for(ion in MRM :> projectAs(as.object)) {
	for(raw in list.files(mzML, pattern = "*.mzML")) {
		let TIC = as.object(extract.ions(raw, ion, tolerance)[1])$chromatogram;
		let data = list();
		
		data[[ion$accession]] <- TIC;
		
		data
		:> MRM.chromatogramPeaks.plot(
			fill              = FALSE, 
			gridFill          = "white", 
			lineStyle         = "stroke: black; stroke-width: 5px; stroke-dash: solid;",
			size              = [1600, 900],
			relativeTimeScale = NULL
		)
		:> save.graphics(file = `${export}/${basename(raw)}/${ion$accession}.png`)
		;
	}
}