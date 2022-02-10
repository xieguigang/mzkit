\name{calc.zerocoef}
\alias{calc.zerocoef}
\title{Correction for warping coefficients when using zeropadding}
\description{This function calculates the warping coefficients for the
  original range of the data, based on the warping of zero-filled
  data. Only needed when zeros are added in the beginning of the signal.} 
\usage{
calc.zerocoef(coef, zeros)}
\arguments{
  \item{coef}{vector of warping coefficients of a PTW-calculation on a
    set of signals with zeros added to the beginning of the signal} 
  \item{zeros}{the number of zeros added}
}
\value{a vector containing the corrected warping coefficients}

\seealso{
  \code{\link{padzeros}}
  \code{\link{calc.multicoef}}
  }
\references{
  Bloemberg, T.G., et al. (2010) "Improved parametric time warping for Proteomics", Chemometrics and Intelligent Laboratory Systems, \bold{104} (1), 65 -- 74.
}  
\examples{
data(gaschrom)
gaschrom.zf <- padzeros(gaschrom, 250)
ref <- gaschrom[1,]
samp <- gaschrom[16,]
ref.zf <- gaschrom.zf[1,]
samp.zf <- gaschrom.zf[16,]
gaschrom.ptw <- ptw(ref.zf, samp.zf)
layout(matrix(1:2,2,1, byrow=TRUE))
plot(gaschrom.ptw)
corr.coef <- calc.zerocoef(gaschrom.ptw$warp.coef, 250)
gaschrom.ptw2 <- ptw(ref, samp, init.coef = corr.coef, try = TRUE)
plot(gaschrom.ptw2)
}

\author{Jan Gerretzen}
\keyword{manip}
