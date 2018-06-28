#' Identify unknown metabolite by metaDNA algorithm
#'
#' @description
#'
#' @param identify A \code{list} object with two members:
#'
#'      \code{identify = list((data.frame) meta.KEGG, (list) peak_ms2)}
#'
#'      where \code{meta.KEGG} is a data.frame object that should contains
#'      a KEGG id column, and a index column to read \code{peak_ms2} list.
#'      \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'      least two column: \code{mz} and \code{into}
#'
#' @param unknown A \code{list} object with two members:
#'
#'      \code{unknown = list((data.frame) peaktable, (list) peak_ms2)}
#'
#'      where \code{peaktable} is a data.frame object that contains the MS1
#'      information: \code{mz} and \code{rt}, and a additional column index
#'      to read \code{peak_ms2} list.
#'      \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'      least two column: \code{mz} and \code{into}
#'
#' @return A \code{identify} parameter data structure like metabolite identify
#'      result for \code{unknown} parameter input data
#'
metaDNA <- function(identify, unknown, meta.KEGG) {

}
