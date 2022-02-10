\name{wcc}
\alias{wcc}
\alias{wac}
\alias{wcc.st}
\alias{wac.st}
\title{
  Weighted auto- and cross-correlation measures
}
\description{Functions to calculate weighted auto- and cross-correlation
  measures. The wcc is a suitable measure for the similarity of two
  patterns when features may be shifted. Identical patterns lead to a
  wcc value of 1.

  Functions \code{wcc} and \code{wac} are meant for profile data
  (intensities measured at equidistant time points), whereas
  \code{wcc.st} and \code{wac.st} are meant for peak lists. In general,
  wcc values calculated for profiles will be higher since they will also
  include the large similarity in the empty spaces, i.e., parts of the
  profiles where no peaks are present (and that will appear to be
  perfectly aligned), whereas the peak-based version concentrates only
  on the peaks.
}
\usage{
wcc(pattern1, pattern2, trwdth, wghts = NULL, acors1 = NULL, acors2 = NULL)
wac(pattern1, trwdth, wghts = NULL)
}
\arguments{
  \item{pattern1, pattern2}{input patterns, typically spectra. Vectors}
  \item{trwdth}{triangle width, given in the number of data points for
    the profile functions, and in the actual retention times for the
    stick-based warpings.}
  \item{wghts}{optional weights vector, will be calculated from triangle
    width if necessary. Sometimes it is more efficient to
    pre-calculate it and give it as an argument}
  \item{acors1, acors2}{autocorrelations of the input patterns. If not
    provided, they are calculated}
}
\details{Functions \code{wcc} and \code{wac} are defined such that the
  triangle width stands for  
  the number of points on one side of \strong{and including} the current 
  point. Thus, a \code{trwdth} of 0 signifies a non-existent triangle and 
  results in an error; a \code{trwdth} equal to 1 only includes the current
  point with weight 1 and no neighbouring points. For the stick-based
  equivalents, the units of the time axis are used for the triangle width.
}
\value{One number, the weighted autocorrelation or crosscorrelation}
\references{
  de Gelder, R., Wehrens, R. and Hageman, J.A. (2001)
  "A generalized expression for the similarity of spectra: Application to powder diffraction pattern classification",
  Journal of Computational Chemistry, \bold{22}, 273 -- 289. 
}
\author{Ron Wehrens}
\examples{
data(gaschrom)
wcc(gaschrom[1,], gaschrom[2,], 20)

wcc.st(gaschrom.st[[1]], gaschrom.st[[2]], 20)
}
\keyword{manip}
