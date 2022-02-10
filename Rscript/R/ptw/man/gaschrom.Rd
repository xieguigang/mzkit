\name{gaschrom}
\docType{data}
\alias{gaschrom}
\alias{gaschrom.st}
\title{16 calibration GC traces}
\description{
  The object \code{gaschrom} contains 16 calibration GC traces, measured
  at 5,000 time points. A peak-picked version is available as object
  \code{gaschrom.st} (see example).
}
\usage{
data(gaschrom)
}
\source{Claire Boucon, Unilever}
\references{
  Eilers, P.H.C. (2004) "Parametric Time Warping", Analytical Chemistry, \bold{76} (2), 404 -- 411.
}
\examples{
data(gaschrom)

## the gaschrom.st object has been generated in the following way:
\dontrun{
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

gaschrom <- t(apply(gaschrom, 1, baseline.corr))
gaschrom.st <- lapply(1:nrow(gaschrom), 
                      function(ii) 
                      pick.peaks(gaschrom[ii,], span = 11))
## remove peaks with an intensity below 10
gaschrom.st <- lapply(gaschrom.st,
                      function(pk)
                      pk[pk[,2] >= 10,])
}
plot(gaschrom[1,], type = "l", xlim = c(3000, 3500), ylim = c(0, 200))
abline(h = 10, lty = 2, col = 2)
abline(v = gaschrom.st[[1]], col = 4)
}
\keyword{datasets}
