imports ["metadb", "massbank", "math"] from "mzkit";

#' Load lipidmaps data repository from internal data pack
#' 
#' @return a data seuqnece of lipidmaps metadata
#' 
const lipidmaps_repo as function() {
    const filepath = system.file("data/LIPIDMAPS", package = "mzkit");
    const repo = massbank::read.lipidmaps(filepath);

    repo;
}

