\name{lcms}
\docType{data}
\alias{lcms}
\alias{time}
\alias{mz}
\alias{lcms.pks}
\title{Parts of 3 proteomic LC-MS samples}
\description{
  The \code{lcms} data consists of a 100 x 2000 x 3 array \code{lcms}, a
  vector \code{time} of length 2000 and a vector \code{mz} of length 100. The
  LC-MS data in the array are a subset (samples 1, 2 and 5) of a larger
  set measured on a tryptic digest of E. coli proteins (see
  \code{source} section). Separate objects \code{mz} and \code{rt} give
  the values for the first two axis.
  
  Peak picking leads to the object ldms.pks (see example section).
}
\usage{
data(lcms)
}
\source{Nijmegen Proteomics Facility, Department of Laboratory Medicine,
  Radboud University Nijmegen Medical Centre. Data available (in
  different formats) at
  http://www.cac.science.ru.nl/research/data/ecoli/}
\references{
  Bloemberg, T.G., et al. (2010)
  "Improved parametric time warping for Proteomics", Chemometrics and
  Intelligent Laboratory Systems, \bold{104} (1), 65 -- 74.
}
\examples{
## the lcms.pks object is generated in the following way:
\dontrun{
data(lcms)
pick.peaks <- function(x, span) {
  span.width <- span * 2 + 1
  loc.max <- span.width + 1 -
      apply(embed(x, span.width), 1, which.max)
  loc.max[loc.max == 1 | loc.max == span.width] <- NA
  
  pks <- loc.max + 0:(length(loc.max)-1)
  pks <- pks[!is.na(pks)]
  pks.tab <- table(pks)
  
  pks.id <- as.numeric(names(pks.tab)[pks.tab > span])
  
  cbind(rt = pks.id, I = x[pks.id])
}

## bring all samples to the same scale, copied from ptw man page
lcms.scaled <- aperm(apply(lcms, c(1,3), 
                           function(x) x/mean(x) ), c(2,1,3))
lcms.s.z <- aperm(apply(lcms.scaled, c(1,3), 
                        function(x) padzeros(x, 250) ), c(2,1,3))
lcms.pks <- lapply(1:3,
                   function(ii) {
                     lapply(1:nrow(lcms.s.z[,,ii]),
                            function(jj)
                            cbind("mz" = jj,
                                  pick.peaks(lcms.s.z[jj,,ii], 5)))
                   })
}}
\keyword{datasets}
