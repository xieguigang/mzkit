imports "cellsPack" from "mzkit";

const pack_singleCells = function(rawdata, tag = NULL) {
    if (dir.exists(rawdata)) {
        if (is.empty(tag)) {
            tag = basename(rawdata);
        }

        rawdata <- list.files(rawdata, ["*.mzPack"]);
    }

    rawdata 
    |> lapply(file -> open.mzpack(file))
    |> cellsPack::pack_cells()
    ;
}