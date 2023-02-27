imports "mzweb" from "mzkit";
imports "visual" from "mzplot";

#' Load spectrum tree from raw data files
#' 
#' @param files the file path collection of the raw data files. 
#' 
let loadTree = function(files as string) {

}

#' Convert to mzpack data object
#' 
#' @description this function is a unify method for Convert 
#'    ``*.mzXML``/``*.mzML``/``*.raw`` to mzpack data object
#' 
#' @param file a single character string for the source raw data
#' 
let convertToMzPack = function(file as string) {
    file 
    |> open.mzpack(file)
    # and then set thumbnail
    # setThumbnail function returns the mzpack data object
    # itself
    |> setThumbnail(mzpack -> raw_scatter(mzpack))
    ;
}