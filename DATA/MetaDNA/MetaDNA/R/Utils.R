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
