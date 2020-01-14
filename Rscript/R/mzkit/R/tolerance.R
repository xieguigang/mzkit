#Region "Microsoft.ROpen::6fae11bed7a2c40ddf79d286e61fbcb1, tolerance.R"

    # Summaries:

    # PPM <- function(measured, actualValue) {...
    # tolerance.deltaMass <- function(da = 0.3) {...
    # tolerance.ppm <- function(ppm = 20) {...
    # assert.deltaMass <- function(da = 0.3) {...
    # assert.ppm <- function(ppm = 20) {...
    # tolerance <- function(threshold = 0.3, method = c("da", "ppm")) {if (method[1] == "da") {...

#End Region

#' PPM value between two mass value
#'
PPM <- function(measured, actualValue) {
	# 2018-7-8 without abs function for entir value, this may cause bugs in metaDNA
	# for unknown query when actualValue is negative

    # |(measure - reference)| / measure * 1000000
    abs(((measured - actualValue) / actualValue) * 1000000);
}

#' Tolerance in Mass delta mode
#'
#' @param da The mass delta value. By default if two mass value which
#'           their delta value is less than \code{0.3da}, then
#'           the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
#'
tolerance.deltaMass <- function(da = 0.3) {
    describ <- sprintf("%s(da)", da);

    function(a, b) {
        err  <- abs(a - b);
        test <- err <= da;

        list(error     = err,
             valid     = test,
             describ   = describ,
             threshold = da
        );
    }
}

#' Tolerance in PPM mode
#'
#' @param ppm The mass ppm value. By default if two mass value which
#'            their ppm delta value is less than \code{20ppm}, then
#'            the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
#'
tolerance.ppm <- function(ppm = 20) {
    describ <- sprintf("%s(ppm)", ppm);

    function(a, b) {
        err  <- PPM(a, b);
        test <- err <= ppm;

        list(error     = err,
             valid     = test,
             describ   = describ,
             threshold = ppm
        );
    }
}

assert.deltaMass <- function(da = 0.3) {
    function(a, b) abs(a-b) <= da;
}

assert.ppm <- function(ppm = 20) {
    function(a, b) PPM(a, b) <= ppm;
}

#' \code{m/z} tolerance helper
#'
#' @return This function returns a list object with members:
#'    \enumerate{
#'        \item \code{threshold} The tolerance threshold.
#'        \item \code{method} The tolerance method name, which could be \code{da} or \code{ppm}.
#'        \item \code{assert} The tolerance assert method function, which could be
#'              \code{\link{assert.deltaMass}} or \code{\link{assert.ppm}} based on the \code{method}
#'              parameter value.
#'        \item \code{is.low.resolution} The tolerance \code{assert} method is a low resolution
#'              method? \code{da} method is \code{TRUE}, and \code{ppm} method is \code{FALSE}.
#'    }
#'
tolerance <- function(threshold = 0.3, method = c("da", "ppm")) {
    if (method[1] == "da") {
        assert = assert.deltaMass(threshold);
        is.low.resolution = TRUE;
    } else if (method[1] == "ppm") {
        assert = assert.ppm(threshold);
        is.low.resolution = FALSE;
    } else {
        stop(sprintf("Unknown tolerance method: '%s'.", method[1]));
    }

    resolution <- if (is.low.resolution) {
        "Low resolution";
    } else {
        "High resolution";
    }

    list(
        threshold         = threshold,
        method            = method[1],
        assert            = assert,
        is.low.resolution = is.low.resolution,
        toString          = sprintf("%s m/z tolerance with threshold %s(%s).", resolution, threshold, method[1])
    );
}
