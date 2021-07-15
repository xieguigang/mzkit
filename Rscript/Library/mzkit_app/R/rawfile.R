imports "mzweb" from "mzkit";
imports "visual" from "mzplot";

#' Load spectrum tree from raw data files
#' 
#' @param files the file path collection of the raw data files. 
#' 
let loadTree as function(files as string) {

}

#' Convert to mzpack data object
#' 
#' @description this function is a unify method for Convert 
#'    ``*.mzXML``/``*.mzML``/``*.raw`` to mzpack data object
#' 
let convertToMzPack as function(file as string) {
    const mzpack = open.mzpack(file);

    # and then set thumbnail
    # setThumbnail function returns the mzpack data object
    # itself
    mzpack 
    |> setThumbnail(raw_scatter(mzpack))
    ;
}