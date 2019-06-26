
#' A unified method for create node ID
#'
#' @details Create debug data of metaDNA network elongation.
#'    The input seed parameter should have such data structure:
#'    \enumeration{
#'       \item \code{feature} is the ms1 feature ID
#'       \item \code{KEGG} is the kegg compound id that current seed object identified as
#'       \item \code{ref} is the ms2 spectrum matrix reference index value, which should contains two member: \code{file} and \code{scan}
#'    }
#' 
#' @param seed The identified metaDNA seed data. 
#' 
trace.node <- function(seed) {
	sprintf("%s|%s#%s#%s", seed$KEGG, seed$feature, seed$ref$file, seed$ref$scan);
}