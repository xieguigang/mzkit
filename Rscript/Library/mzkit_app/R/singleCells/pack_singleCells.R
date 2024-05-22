const pack_singleCells = function(rawdata) {
    if (dir.exists(rawdata)) {
        rawdata = list.files(rawdata, ["*.mzPack"]);
    }

    rawdata 
    |> lapply(file -> open.mzpack(file))
    |> 
}