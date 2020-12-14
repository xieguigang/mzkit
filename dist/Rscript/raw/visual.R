imports "visual" from "mzkit.plot";
imports ["assembly", "mzweb"] from "mzkit";
imports "charts" from "R.plot";

#'
#' @param folder the output folder path
#'
let runPlot as function(rawfiles, folder) {
	let overlaps           = new overlaps();
	let chromatogram       = NULL;
	let fileName as string = NULL;
	
	print(`get ${length(rawfiles)} sample files!`);
	print(`image plots will be saved at location ${folder}.`);
	
	for(file in rawfiles) {
		fileName     = basename(file, withExtensionName = TRUE);	
		chromatogram = plotSingleFile(file);		
		
		overlaps :> add(fileName, chromatogram);
		
		print(fileName);
	}
	
	plot(overlaps, bpc = FALSE, opacity = 40) :> save.graphics(file = `${folder}/TIC_overlap.png`); 
	plot(overlaps, bpc = TRUE,  opacity = 40) :> save.graphics(file = `${folder}/BPC_overlap.png`); 
	
	print("job done!");
}

#' plot single raw file and returns the chromatogram data for overlap plot.
#'
let plotSingleFile as function(file) {
	let ticplot as string     = `${dirname(file)}/${basename(file)}_TIC.png`;
	let bpcplot as string     = `${dirname(file)}/${basename(file)}_BPC.png`;
	let scatterplot as string = `${dirname(file)}/${basename(file)}_rawscatter.png`;
	let chromatogram          = load.chromatogram(raw.scans(file)); 
	let fileName as string    = basename(file, withExtensionName = TRUE);
	
	ms1.scans(file, centroid = "da:0.3")
	:> raw_scatter
	:> save.graphics(file = scatterplot)
	;
	
	chromatogram
	:> plot(bpc = FALSE, name = fileName)
	:> save.graphics(file = ticplot)
	;
	
	chromatogram
	:> plot(bpc = TRUE, name = fileName)
	:> save.graphics(file = bpcplot)
	;
	
	chromatogram;
}

list.files(dir    = ?"--data"   || stop("no raw data input folder path provided!"), pattern = "*.mz*ML")
:> runPlot(folder = ?"--output" || (?"--data"))
;