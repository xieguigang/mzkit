#' Load lipidmaps data repository from internal data pack
#' 
#' @param repofile the file path of the messagepack repository
#'     data file. default file path is the internal data file
#'     in mzkit package. 
#' @param gsea the lipidmaps database should be cast to a gsea background? 
#' 
#' @return a data seuqnece of lipidmaps metadata
#' 
const lipidmaps_repo = function(repofile = system.file("data/LIPIDMAPS.msgpack", package = "mzkit"), 
                                gsea = FALSE,
								category = FALSE) {

    if (!file.exists(repofile)) {
        stop(`no repository data file at location: ${repofile}!`);
    } else {
        # read messagepack repository data file
        #
        massbank::read.lipidmaps(
			file = repofile, 
			gsea_background = gsea, 
			category_model = category
		);
    }
}

#' load the lipidmaps raw database
#' 
#' @param filepath the file path of the lipidmaps database file, ``structures.sdf`` file,
#'    which could be download from the lipidmaps website via the file download url:
#' 
#'    https://lipidmaps.org/files/?file=LMSD&ext=sdf.zip
#' 
const load_LMSD = function(filepath, lazy = FALSE) {
    let dataset = read.SDF(file = filepath, parseStruct = FALSE, 
              lazy = lazy);
    let lipids = as.lipidmaps(dataset, lazy = lazy);
    
    lipids;
}