require(mzkit);

imports "mzDeco" from "mz_quantify";

#' Export peakstable data from the XIC rawdata files
#' 
#' @param files a character vector of the file path of the XIC rawdata files
#' @param mzbins A dataframe object that contains the target ion m/z feature 
#'    set for do extract of the result peaktable data, a data field which is 
#'    named ``mz`` must be included inside this dataframe object.
#' @param peak.width the peak time range of the peaks that could be accepted
#' 
#' @return this function generates a xcms format liked peaktable dataframe object
#'    for the input rawdata files.
#' 
const ms1_peaktable = function(files, mzbins, mzdiff = 0.005, peak.width = [3,90]) {
    let pool = xic_pool(files);
    
    if (is.character(mzbins)) {
        mzbins = read.csv(mzbins, row.names = NULL, check.names = FALSE);
        mzbins = mzbins$mz;
    } else {
        if (is.data.frame(mzbins)) {
            mzbins = mzbins$mz;
        } else {
            mzbins = as.numeric(mzbins);
        }
    }

    print("get mz ion features of the dataset:");
    print(mzbins, max.print = 13);

    print(`run extract peaktable set from ${length(mzbins)} ion m/z features...`);

    return(mz_deco(pool, 
        tolerance = `da:${mzdiff}`, joint = TRUE, 
        peak.width = peak.width, 
        feature = mzbins, 
        parallel = TRUE)
    );
}