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
const ms1_mz_bins = function(files, mzdiff = 0.001) {
    let xic_data = lapply(files, path -> readBin(path, what = "mz_group"));
    let mz = lapply(xic_data, function(pack, i) {
        # some rawdata file may contains ms2 spectrum only
        if (length(pack) == 0) {
            NULL;
        } else {
            data.frame(
                mz        = [pack]::mz,
                TIC       = [pack]::TIC,
                maxinto   = [pack]::MaxInto,
                row.names = `#${i}-${1:length(pack)}`
            );
        }
    });

    # merge the ion dataframes from each files
    let mzraw = bind_rows(mz);

    # make centroid 
    mzraw <- libraryMatrix(data.frame(
        mz   = mzraw$mz, 
        into = mzraw$TIC));
    mzraw <- centroid(mzraw, tolerance = `da:${mzdiff}`, intoCutoff = 0);
    mzraw <- as.data.frame(mzraw);

    print("get ion features:");
    print(mzraw, max.print = 6);

    return(mzraw);
}

