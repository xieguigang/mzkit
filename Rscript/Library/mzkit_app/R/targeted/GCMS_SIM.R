imports ["GCMS", "Linears"] from "mz_quantify";
imports "visualPlots" from "mz_quantify";
imports "assembly" from "mzkit";

#' Create reference linears for GCMS sim data
#' 
#' @param maxDeletions the max number of the outlier points that removes from the points data of linear fitting.
#' @param peakwidth the rt range of the ion peak in TIC data 
#' @param rtshift the max rt shift value between the raw sample reference data and the rt value in msl ions information.
#' @param calfiles a collection of the reference sample data files theirs file path.
#' 
#' @return a collection of data linears
#' 
const GCMS_linears = function(contentTable, mslIons, calfiles as string, peakwidth = [5, 13], rtshift = 30, maxDeletions = 2) {
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

#' Create content levels table
#' 
#' @param calfiles the reference raw data files theirs file path.
#' 
#' @details the file names of the linear reference samples should be 
#'     the content value like: ``100ppm``, ``50ppb``, etc.
#'     This method will parse the contents value from the ``calfiles`` 
#'     file name. 
#'
const GCMS_contentTable = function(mslIons, calfiles as string) {
	const contents = parseContents(calfiles);

	print("contents of reference samples in ppb unit:");
	str(contents);

	contentTable(read.msl(MSLIons, "Minute"), contents, IS = "IS");
}

const GCMS_quantify = function(linears, sim, sampleData) {
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

const GCMS_linearReport = function(sim, ions, quantify, calfiles as string, output_dir as string = "./") {
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