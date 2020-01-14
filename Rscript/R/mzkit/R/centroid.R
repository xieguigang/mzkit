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
centroid.2 <- function(profile, peakwidth = 0.3, angle.threshold = 0.5) {
    if (!(c("mz", "into") %in% colnames(profile))) {
        stop("Invalid prpfile spectra data matrix object!");
    }

    mz <- as.numeric(as.vector(profile[, "mz"]));
    into <- as.numeric(as.vector(profile[, "into"]));

	# reorder in asc order
	i <- order(mz);
	mz <- mz[i];
	into <- into[i];

	i <- (into / max(into)) >= 0.05;
	mz <- mz[i];
	into <- into[i];

    # reduce the spectra data size from profiles data to centroid data
    # algorithm by peak detection
    #
    # https://github.com/xieguigang/mzkit/blob/master/src/mzmath/ms2_math-core/Chromatogram/AccumulateROI.vb
    #
    # due to the reason of the ms2 profiles peaks is not overlapping
    # each other
    accumulates <- peak.accumulateLine(into);
    windowSlices <- slide.windows(win_size = 2, step = 1, mz = mz, into = into);

    cmz   <- c();
    cinto <- c();
    bmz   <- c();
    binto <- c();

    for(slide in windowSlices) {
        p1 <- c(slide$mz[1], slide$into[1]);
        p2 <- c(slide$mz[2], slide$into[2]);
        a <- angle(p1, p2);

        if (abs(a - 360) <= angle.threshold) {
			if(length(bmz) > 0) {
            # we get a spectra peak
            i <- which.max(binto);
            cmz <- append(cmz, bmz[i]);
            cinto <- append(cinto, binto[i]);			
			}

            bmz   <- c();
            binto <- c();
        } else {
            if (is.null(bmz)) {
                bmz <- slide$mz;
                binto <- slide$into;
            } else {
                bmz <- append(bmz, slide$mz[2]);
                binto <- append(binto, slide$into[2]);
            }
        }
    }

    if (length(bmz) > 0) {
        # we get a spectra peak
        i <- which.max(binto);
        cmz <- append(cmz, bmz[i]);
        cinto <- append(cinto, binto[i]);
    }

    # we get a ms2 spectra peaks data in centroid mode
    data.frame(mz = cmz, into = cinto);
}

angle <- function(p1, p2) {
    xydiff <- p2 - p1;
    a <- atan2(xydiff[2], xydiff[1]);
    a <- a * 180 / pi;

    abs(180 - (a - 90));
}

peak.accumulateLine <- function(into) {
    sum.all <- sum(into);
    accumulates <- reduce(into, aggregate = function(a, b) a + b) / sum.all;
    accumulates;
}
