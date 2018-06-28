#' Identify unknown metabolite by metaDNA algorithm
#'
#' @description How to: The basic idea of the \code{MetaDNA} algorightm is
#'      using the identified ms2 data to align the unknown ms2 data.
#'
#' @param identify A \code{list} object with two members:
#'
#'      \code{identify = list((data.frame) meta.KEGG, (list) peak_ms2)}
#'
#'      where:
#'
#'      \enumerate{
#'         \item \code{meta.KEGG} is a data.frame object that should contains
#'               a KEGG id column, and a index column to read \code{peak_ms2} list.
#'         \item \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'               least two column: \code{mz} and \code{into}
#'      }
#'
#' @param unknown A \code{list} object with two members:
#'
#'      \code{unknown = list((data.frame) peaktable, (list) peak_ms2)}
#'
#'      where:
#'
#'      \code{peaktable} is a data.frame object that contains the MS1
#'      information: \code{mz} and \code{rt}, and a additional column index
#'      to read \code{peak_ms2} list.
#'
#'      \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'      least two column: \code{mz} and \code{into}
#'
#' @param precursor_type By default is positive mode with the \code{H+} adduct for
#'      search unknown metabolites.
#'
#' @return A \code{identify} parameter data structure like metabolite identify
#'      result for \code{unknown} parameter input data
#'
metaDNA <- function(identify, unknown, meta.KEGG, ms2.align, precursor_type = "[M+H]+") {
  # 1. Find all of the related KEGG compound by KEGG reaction link for
  #    identify metabolites
  # 2. Search for unknown by using ms1 precursor_m/z compare with the
  #    KEGG compound molecule weight in a given precursor_type mode.


}
