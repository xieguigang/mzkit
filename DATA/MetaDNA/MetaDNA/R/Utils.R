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
removes.empty_KEGG <- function(d) {
	KEGG <- d[, "KEGG"] %=>% as.vector;
	d[!Strings.Empty(KEGG, TRUE), ];
}

#' Tolerance in Mass delta mode
#'
#' @param da The mass delta value. By default if two mass value which
#'           their delta value is less than \code{0.3da}, then
#'           the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
tolerance.deltaMass <- function(da = 0.3) {
  function(a, b) abs(a - b) <= da;
}

#' Tolerance in PPM mode
#'
#' @param ppm The mass ppm value. By default if two mass value which
#'            their ppm delta value is less than \code{20ppm}, then
#'            the predicate will be true.
#'
#' @return Function returns a lambda function that can be using for
#'         tolerance predication.
tolerance.ppm <- function(ppm = 20) {
  function(a, b) PPM(a, b) <= ppm;
}

#' PPM value between two mass value
PPM <- function(measured, actualValue) {
  # |(measure - reference)| / measure * 1000000
  (abs(measured - actualValue) / actualValue) * 1000000;
}
