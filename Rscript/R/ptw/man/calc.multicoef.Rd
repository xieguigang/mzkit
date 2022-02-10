\name{calc.multicoef}
\alias{calc.multicoef}
\title{Calculation of warping coefficients when applying more than one
  warping function successively} 
\description{Applying two (or more) warping function after each other
  can be described with one warping function of a higher warping
  degree. This function provides the coefficients of this higher degree
  warping function.} 
\usage{calc.multicoef(coef1, coef2)}
\arguments{
  \item{coef1}{vector containing the warping coefficients of the first
    applied warping function} 
  \item{coef2}{vector containing the warping coefficients of the second
    applied warping function} 
}

\details{This function uses Pascal's simplex to calculate the new
  warping coefficients. 

When applying three warping functions successively (first a, then b and
finally c - here a, b and c are vectors of warping coefficients), first
calculate the new coefficients for b and c, and afterwards the
coefficients for a with these new coefficients. So the coefficients for
the total warping function can be calculated via \code{calc.multicoef(a,
  calc.multicoef(b, c))}. 
}

\value{a vector containing the corrected warping coefficients}

\seealso{
  \code{\link{calc.zerocoef}}
}
\references{
  Bloemberg, T.G., et al. (2010) "Improved parametric time warping for Proteomics", Chemometrics and Intelligent Laboratory Systems, \bold{104} (1), 65 -- 74.
}
\examples{
data(gaschrom)
ref <- gaschrom[1,]
samp <- gaschrom[16,]
coef1 <- c(100,1.1, 1e-5)
coef2 <- c(25, 0.95, 3.2e-5)
gaschrom.ptw <- ptw(ref, samp, init.coef = coef1, try = TRUE)
ref.w <- gaschrom.ptw$reference
samp.w <- gaschrom.ptw$warped.sample
samp.w[is.na(samp.w)] <- 0
gaschrom.ptw2 <- ptw(ref.w, samp.w, init.coef = coef2, try = TRUE)
plot(c(gaschrom.ptw2$warped.sample), type = "l")

corr.coef <- calc.multicoef(coef1, coef2)
gaschrom.ptw3 <- ptw(ref, samp, init.coef = corr.coef, try = TRUE)
lines(c(gaschrom.ptw3$warped.sample), col = 2, lty = 2)
}

\author{Jan Gerretzen}

\keyword{manip}
