require(mzkit);

#' title: convert raw files to mzpack 
#' author: xieguigang
#'
#' description: A commandline script for convert raw data files to 
#'    mzpack data files in batch.
#'    

[@info "A directory path that contains the raw data files that needs to be convert."]
const sourceDir as string = ?"--scan"   || stop("You must provides a directory path for load the raw data files!");
[@info "A file extension name list with ',' or ';' symbol as delimiter."]
const filter as string    = ?"--filter" || "*.raw;*.mzXML;*.mzML";
[@info "A directory for output the converted mzPack raw data files."]
const outputdir as string = ?"--output" || sourceDir;

const rawfiles as string = sourceDir 
|> list.files(pattern = strsplit(filter, "[;,]"))
;

for(file in rawfiles) {
	cat(basename(file));
	
	file
	|> convertToMzPack
	|> write.mzPack(
		file = `${outputdir}/${basename(file)}.mzPack`
	)
	;
	
	cat(" ... done!\n");
}

print("all data file has been converted!");