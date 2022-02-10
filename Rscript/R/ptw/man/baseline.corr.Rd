\name{baseline.corr}
\alias{baseline.corr}
\title{Baseline Correction using asymmetric least squares}
\description{This function estimates a baseline using asymmetric least
  squares and subtracts it from the data.} 
\usage{
baseline.corr(y, ...)
}
\arguments{
  \item{y}{signal(s) to correct. This can be a vector (containing one
    signal) or a matrix of signals(one signal per row)}
  \item{\dots}{other arguments to the \code{asysm} function.}
}
\value{
  \item{ycorr}{baseline corrected signal(s): a vector or a matrix of the
    same dimension as the input signal(s)}
}
\references{
  Eilers, P.H.C. (2004) "Parametric Time Warping", Analytical Chemistry, \bold{76} (2), 404 -- 411.
}
\author{Paul Eilers, Jan Gerretzen}
\examples{
data(gaschrom)
plot(gaschrom[1,], type = "l", ylim = c(0, 100))
lines(baseline.corr(gaschrom[1,]), col = 2)
}
\keyword{manip}
