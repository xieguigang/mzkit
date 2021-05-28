require(mzkit);

#' title: convert raw files to mzpack 
#' author: xieguigang
#'
#' description: A commandline script for convert raw data files to 
#' mzpack data files in batch.
#'    

[@info "A directory path that contains the raw data files that needs to be convert."]
const sourceDir as string = ?"--scan"   || stop("You must provides a directory path for load the raw data files!");
const israwFile = file.exists(sourceDir);
[@info "A file extension name list with ',' or ';' symbol as delimiter."]
const filter as string    = ?"--filter" || "*.raw;*.mzXML;*.mzML";
[@info "A directory for output the converted mzPack raw data files."]
const outputdir as string = ?"--output" || ifelse(israwFile, dirname(sourceDir), sourceDir);

const convertRawTask as function(file) {
	cat(basename(file));
	
	file
	|> convertToMzPack
	|> write.mzPack(
		file = `${outputdir}/${basename(file)}.mzPack`
	)
	;
	
	cat(" ... done!\n");
}

if (israwFile) {
	convertRawTask(sourceDir);
} else {
	const rawfiles as string = sourceDir 
	|> list.files(pattern = strsplit(filter, "[;,]"))
	;

	for(file in rawfiles) {
		convertRawTask(file);
	}
}

print("all data file has been converted!");