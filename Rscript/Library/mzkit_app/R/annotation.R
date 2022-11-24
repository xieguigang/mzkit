imports ["metadb", "massbank", "math"] from "mzkit";
imports "metadna" from "mzDIA";

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
                                gsea = FALSE) {

    if (!file.exists(repofile)) {
        stop(`no repository data file at location: ${repofile}!`);
    } else {
        # read messagepack repository data file
        #
        const repo = massbank::read.lipidmaps(repofile, gsea_background = gsea);
        repo;
    }
}

const kegg_compounds = function(precursors = ["[M]+", "[M+H]+", "[M+H-H2O]+"], 
                                mzdiff     = "ppm:20", 
                                repofile   = system.file("data/KEGG_compounds.msgpack", package = "mzkit")) {

    if (!file.exists(repofile)) {
        stop(`no repository data file at location: ${repofile}!`);
    } else {
        # read messagepack repository data file
        #
        repofile
        |> kegg.library()
        |> annotationSet()
        ;        
    }
}