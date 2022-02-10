\name{bestref}
\alias{bestref}
\title{Identification of optimal reference}
\description{This function calculates a similarity matrix and returns
  the sample number that is most similar to all other samples. This is
  possibly preferable as a reference sample since warping then may be
  kept to a minimum. Either RMS or WCC may be used as similarity
  functions.} 
\usage{bestref(x, optim.crit = c("WCC", "RMS"),
               trwdth=20, wghts = NULL, smooth.param = 0)
}
\arguments{
  \item{x}{data matrix or array of signals, specified row-wise. In case of an array, the third dimension should differentiate between the different samples}
  \item{optim.crit}{either \code{"WCC"} or \code{"RMS"}}
  \item{trwdth}{the width of the triangle in the WCC criterion, given as
    a number of data points. Default: 20}
  \item{wghts}{Optional weights vector in the WCC criterion; will be
    calculated from the triangle width if necessary. Sometimes it is
    more efficient to pre-calculate it and give it as an argument}
  \item{smooth.param}{smoothing parameter for smoothing the signal when
    \code{optim.crit} equals \code{"RMS"}. If no smoothing is required,
    set this to 0}
}
\value{A list containing two elements:
    \item{best.ref}{the index of the best reference(s)}
    \item{crit.values}{the qualities as measured by either RMS or WCC}
  }
\author{Jan Gerretzen, Ron Wehrens}
\examples{
data(gaschrom)
bestref(gaschrom)
bestref(gaschrom, optim.crit = "WCC", trwdth = 50)
bestref(gaschrom, optim.crit = "RMS")
bestref(gaschrom, optim.crit = "RMS", smooth.param = 1e5)
}
\keyword{manip}
