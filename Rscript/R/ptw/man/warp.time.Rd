\name{warp.time}
\alias{warp.time}
\title{Transform time according to a given warping function}
\description{Warp time points according to a warping function.}
\usage{warp.time(tp, coef)}
\arguments{
  \item{tp}{A vector of time points, not necessarily equidistant.}
  \item{coef}{The coefficients of the parametric time warping function:
    the first coefficient is the constant shift, the second the linear
    stretch etcetera.}
}
\value{The function returns a vector of warped time points.}
\references{Wehrens, R. et al. (2015)
  "Fast parametric warping of peak lists",
  Bioinformatics. DOI: 10.1093/bioinformatics/btv299.}
\author{Ron Wehrens}
\examples{
time <- 1:100
## simple shift and compression
warp.time(time, c(-10, .99))
}
\keyword{manip}
