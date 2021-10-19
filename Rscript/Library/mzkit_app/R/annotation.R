imports ["metadb", "massbank", "math"] from "mzkit";

#' Load lipidmaps data repository from internal data pack
#' 
#' @param repofile the file path of the messagepack repository
#'     data file. default file path is the internal data file
#'     in mzkit package. 
#' 
#' @return a data seuqnece of lipidmaps metadata
#' 
const lipidmaps_repo as function(repofile = system.file("data/LIPIDMAPS.msgpack", package = "mzkit")) {
    if (!file.exists(repofile)) {
        stop(`no repository data file at location: ${repofile}!`);
    } else {
        # read messagepack repository data file
        #
        const repo = massbank::read.lipidmaps(repofile);
        repo;
    }
}