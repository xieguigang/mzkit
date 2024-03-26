imports "mzDeco" from "mz_quantify";

#' Extract the ion m/z features
#' 
#' @param files a character vector of file path of the XIC data files which 
#'      are extract from the rawdata file via the ``ms1_xic_bins`` function.
#' 
#' @return a dataframe object that contains the ion m/z features, data 
#'      fields are included inside this result table:
#' 
#'    1. mz: the ion m/z feature numeric vector
#'    2. into: the max intensity value of current ion feature
#' 
const ms1_mz_bins = function(files) {
    let xic_data = lapply(files, path -> readBin(path, what = "mz_group"));
    let mz = lapply(xic_data, function(pack, i) {
        data.frame(
            mz = [pack]::mz,
            TIC = [pack]::TIC,
            maxinto = [pack]::MaxInto,
            row.names = `#${i}-${1:length(pack)}`
        );
    });

    let mzraw = NULL;

    for(m in mz) {
        mzraw <- rbind(m, mzraw);
        NULL;
    }

    # make centroid 
    mzraw <- libraryMatrix(data.frame(
        mz   = mzraw$mz, 
        into = mzraw$TIC));
    mzraw <- centroid(mzraw, tolerance = "da:0.005", intoCutoff = 0);
    mzraw <- as.data.frame(mzraw);

    print("get ion features:");
    print(mzraw, max.print = 6);

    return(mzraw);
}

