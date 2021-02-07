imports ["GCMS", "Linears"] from "mzkit.quantify";
imports "visualPlots" from "mzkit.quantify";
imports "assembly" from "mzkit";

let GCMS_linears as function(contentTable, mslIons, calfiles as string, peakwidth = [5, 13], rtshift = 30, maxDeletions = 2) {
	const ions = as.quantify.ion(mslIons);
	const sim  = ScanIonExtractor(ions, peakwidth = peakwidth, rtshift = rtshift);

	print("read ions raw data and run linear fitting:");

	const cal     = lapply(calfiles, function(path) peakRaw(sim, read.raw(path)), names = basename);
	const linears = linear_algorithm(contentTable, maxDeletions = 2) :> linears(unlist(cal));

	print("Linear modeeling result of your ions data in reference samples:");
	cat("\n");

	for (line in linears) {
		print(line);
	}

	linears;
}

let GCMS_contentTable as function(mslIons, calfiles as string) {
	const contents = parseContents(calfiles);

	print("contents of reference samples in ppb unit:");
	str(contents);

	contentTable(read.msl(MSLIons, "Minute"), contents, IS = "IS");
}

let GCMS_quantify as function(linears, sim, sampleData) {
	sampleData :> sapply(function(file) {
		print("Run quantification of sample data file:");
		print(file);
		
		linears :> quantify(
			ions             = sim :> peakRaw(read.raw(file)), 
			integrator       = "SumAll", 
			fileName         = file, 
			baselineQuantile = 0
		);
	});
}

let GCMS_linearReport as function(sim, ions, quantify, calfiles as string, output_dir as string = "./") {
	print("create linear report:");

	const plotRaw  = lapply(calfiles, function(path) peakRaw(sim, read.raw(path), chromatogramPlot = TRUE), names = basename);
	const plotIons = lapply(ions, function(ion) {
		const ion_id as string = as.object(ion)$id;
		const filesData = lapply(plotRaw, file -> file[[ion_id]]);
		
		filesData;
	}, names = ion -> as.object(ion)$id)
	;

	print("output html report...");

	report.dataset(linears, quantify, ionsRaw = plotIons)
	:> html()
	:> writeLines(con = `${output_dir}/index.html`)
	;
}