#Region "Microsoft.ROpen::de0a1f3373357fb919ed77121a0ee0c2, Utils.R"

    # Summaries:

    # removes.empty_KEGG <- function(d) {...
    # tolerance.deltaMass <- function(da = 0.3) {...
    # tolerance.ppm <- function(ppm = 20) {...
    # PPM <- function(measured, actualValue) {...

#End Region

#' Removes empty \code{KEGG} row
#'
#' @description Removes all of the rows in a given \code{dataframe}
#'      which its \code{KEGG} column value is string empty.
#'
#' @param d A \code{dataframe} object which contains the metabolite
#'      annotation result data. This dataframe must contains a column
#'      which is named \code{KEGG}.
#'
#' @return A subset of the input \code{dataframe} with all KEGG column
#'     value non-empty.
Delete.EmptyKEGG <- function(dataframe, col.name = "KEGG") {
    KEGG <- dataframe[, col.name] %=>% as.vector;
	test <- !Strings.Empty(KEGG, TRUE);
	
    dataframe[test, ];
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

#' PPM value between two mass value
#'
PPM <- function(measured, actualValue) {
	# 2018-7-8 without abs function for entir value, this may cause bugs in metaDNA
	# for unknown query when actualValue is negative

    # |(measure - reference)| / measure * 1000000
    abs(((measured - actualValue) / actualValue) * 1000000);
}
