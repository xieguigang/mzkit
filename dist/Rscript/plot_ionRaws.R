imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

let plot_ionRaws as function(MRM, mzML, tolerance, export) {
	for(ion in MRM) {
		let chromatograms = lapply(mzML, function(path) {
			as.object(extract.ions(path, ion, tolerance)[1])$chromatogram;
		});

		names(chromatograms) <- basename(mzML);
		
		chromatograms 
		:> MRM.chromatogramPeaks.plot(
			fill              = FALSE, 
			gridFill          = "white", 
			lineStyle         = "stroke: black; stroke-width: 5px; stroke-dash: solid;",
			size              = [1600, 900],
			relativeTimeScale = NULL
		)
		:> save.graphics(file = `${export}/${as.object(ion)$accession}.png`)
		;
	}
}