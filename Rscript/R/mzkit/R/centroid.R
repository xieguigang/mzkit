#' Convert profile data to centroid
#' 
#' @details Convert the profiles spectrum data matrix to centroid data mode.
#'    The matrix data for this function shoule be a dataframe object which 
#'    at least contains 2 column data, where:
#'       \code{profile <- data.frame(mz = mzlist, into = intolist);}
#' 
#'    If the \code{mz} or \code{into} data column is missing, then this function
#'    will throw an exception
#' 
#' @param profile A 2D spectra data matrix in profile mode
#' 
#' @return A 2D spectra data matrix in simple centroid mode.
#'
centroid.2 <- function(profile) {
    if (!(c("mz", "into") %in% colnames(profile))) {
        stop("Invalid prpfile spectra data matrix object!");
    }

    mz <- as.numeric(as.vector(profile[, "mz"]));
    into <- as.numeric(as.vector(profile[, "into"]));

    # reduce the spectra data size from profiles data to centroid data
    # algorithm by peak detection
    #
    # https://github.com/xieguigang/mzkit/blob/master/src/mzmath/ms2_math-core/Chromatogram/AccumulateROI.vb
    #
    # due to the reason of the ms2 profiles peaks is not overlapping 
    # each other

}