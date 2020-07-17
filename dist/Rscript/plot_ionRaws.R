imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";
imports "assembly" from "mzkit.dll";

let plot_ionRaws as function(MRM, mzML, tolerance, export) {
	for(ion in getIonsSampleRaw(MRM, mzML, tolerance)) {
		ion$chromatograms 
		:> MRM.chromatogramPeaks.plot(
			fill              = FALSE, 
			gridFill          = "white", 
			lineStyle         = "stroke: black; stroke-width: 5px; stroke-dash: solid;",
			size              = [1600, 900],
			relativeTimeScale = NULL
		)
		:> save.graphics(file = `${export}/${ion$id}.png`)
		;
	}
}

let getIonsSampleRaw as function(MRM, mzML, tolerance) {
	lapply(MRM, function(ion) {
		let chromatograms = lapply(mzML, function(path) {
			as.object(extract.ions(path, ion, tolerance)[1])$chromatogram;
		});

		names(chromatograms) <- basename(mzML);
	
		list(
			id            = as.object(ion)$accession, 
			chromatograms = chromatograms
		);	
		
	}, names = ion -> as.object(ion)$accession);
}