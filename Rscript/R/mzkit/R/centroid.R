#Region "Microsoft.ROpen::46b8f7145b3fffcc63e247555d9b0b19, R\centroid.R"

    # Summaries:

    # centroid.2 <- function(profile, peakwidth = 0.1, intocutoff = 0.05) {if (!all(c("mz", "into") %in% colnames(profile))) {...

#End Region

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
#' @param peakwidth The spectra peak width in \code{da} unit
#'
#' @return A 2D spectra data matrix in simple centroid mode.
#'
centroid.2 <- function(profile, peakwidth = 0.1, intocutoff = 0.05) {
  if (!all(c("mz", "into") %in% colnames(profile))) {
    stop("Invalid prpfile spectra data matrix object!");
  }

  mz   = as.numeric(as.vector(profile[, "mz"]));
  into = as.numeric(as.vector(profile[, "into"]));

  # reorder in asc order
  i    = order(mz);
  mz   = mz[i];
  into = into[i];

  i    = (into / max(into)) >= intocutoff;
  mz   = mz[i];
  into = into[i];

  mzgroups = numeric.group(mz, assert = function(x, y) abs(x - y) <= peakwidth);
  cmz      = NULL;
  cinto    = NULL;

  for(mzi in mzgroups) {
    i     = (mz >= min(mzi)) & (mz <= max(mzi));
    int   = into[i];
    mzi   = mz[i];
    cmz   = append(cmz, mzi[which.max(int)]);
    cinto = append(cinto, max(int));
  }

  # we get a ms2 spectra peaks data in centroid mode
  data.frame(mz = cmz, into = cinto);
}

#' Convert \code{mzInto} object as data.frame
#' 
#' @param x a spectrum object in \code{mzInto} class.
#' 
#' @return a dataframe with two column fields: 
#'    \code{mz} and \code{into}.
#'
toMsMatrix = function(x) {
  data.frame(
    mz = x@mz,
    into = x@intensity
  );
}

#' Convert MS dataframe as \code{mzInto} class object.
#' 
#' @param x a spectrum data object in dataframe format. the first column
#'    in dataframe should be \code{m/z} value and the second column
#'    in dataframe should be \code{intensity} data.
#' 
toMzInto = function(x) {
  new("mzInto", 
    mz = as.numeric(as.vector(x[, 1])), 
    intensity = as.numeric(as.vector(x[, 2]))
  );
}