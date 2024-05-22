imports "cellsPack" from "mzkit";

#' pack the multiple single cells samples into one dataset.
#' 
#' @param rawdata a character vector of the directory path of 
#'   the rawdata files or a character vector of the raw data 
#'   file path.
#' 
#' @param tag the source tag string for the output mzpack object.
#' 
const pack_singleCells = function(rawdata, tag = NULL) {
    if (dir.exists(rawdata)) {
        if (is.empty(tag)) {
            tag = basename(rawdata);
        }

        rawdata <- list.files(rawdata, ["*.mzPack"]);
    }

    print("pack single cells raw data files:");
    print(basename(rawdata));

    rawdata 
    |> lapply(file -> open.mzpack(file))
    |> cellsPack::pack_cells()
    ;
}