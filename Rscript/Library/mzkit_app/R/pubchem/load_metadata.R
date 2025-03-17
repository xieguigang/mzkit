#' Load pubchem metadata
#' 
#' @details this function load the local pubchem pugview data repository and then 
#'    populates out a collection of the mzkit internal metadata reference object.
#' 
#' @param repo_dir a local directory path that contains the cache data files of the pubchem download data.
#' 
const pugview_repo = function(repo_dir) {
    let local_caches = resolve_repository(repo_dir);
    let meta = local_caches |> sapply(pug -> metadata.pugView(pug));

    meta |> which(m -> !is.null(m));
}