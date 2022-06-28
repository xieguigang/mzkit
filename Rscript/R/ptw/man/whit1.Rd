\name{whit1}
\alias{whit1}
\title{Weighted Whittaker smoothing with a first order finite difference penalty}
\description{
  This function smoothes signals with a finite difference penalty of
  order 1.
}
\usage{
whit1(y, lambda, w)
}
\arguments{
  \item{y}{signal to be smoothed: a vector}
  \item{lambda}{smoothing parameter: larger values lead to more smoothing}
  \item{w}{weights: a vector of same length as y. Default weights are equal to one}
}
\value{smoothed signal: a vector}
\references{
  Eilers, P.H.C. (2004) "Parametric Time Warping", Analytical Chemistry, \bold{76} (2), 404 -- 411.
  
  Eilers, P.H.C. (2003) "A perfect smoother", Analytical Chemistry, \bold{75}, 3631 -- 3636.
}
\author{Paul Eilers, Jan Gerretzen}
\examples{
  data(gaschrom)
  plot(gaschrom[1,], type = "l", ylim = c(0, 100))
  lines(whit1(gaschrom[1,], lambda = 1e1), col = 2)
  lines(whit1(gaschrom[1,], lambda = 1e2), col = 3)
  lines(whit1(gaschrom[1,], lambda = 1e3), col = 4)
  }
\keyword{manip}
