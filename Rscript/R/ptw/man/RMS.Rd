\name{RMS}
\alias{RMS}
\alias{WCC}
\title{Quality criteria for comparing patterns with shifts}
\description{Functions to compare patterns with shifted features. These
  functions compare warped sample patterns to one or more reference
  patterns. RMS returns the usual root-mean-squared difference measure;
  WCC returns 1-wcc, where wcc indicates the weighted
  cross-correlation. Perfect alignment leads to a value of 0 for both
  criteria.
  
  Internal function, not meant to be called directly by the user. In
  particular, note that the identity warping may lead to slightly
  different estimates than a direct comparison of the reference and
  sample signals - a warping even slightly outside the original range of
  \code{1 : ncol(ref)} leads to NA values.}
\usage{
RMS(warp.coef, ref, samp, B, mode)
WCC(warp.coef, ref, samp, B, trwdth = 20, wghts, mode, ref.acors = NULL)
}
\arguments{
  \item{warp.coef}{a vector of warping coefficients}
  \item{ref}{reference signal; a matrix with one or more rows. If the
    number of rows is greater than one, it should equal the number of
    rows in \code{samp}}
  \item{samp}{sample signal; a matrix with one or more rows}
  \item{B}{basis for warping function}
  \item{mode}{either "forward" (new implementation, also used for
  warping peak lists) or "backward" (classical implementation).}
  \item{trwdth}{triangle width for the WCC function, expressed in the number
    of data points}
  \item{wghts}{optional weights vector, will be calculated from triangle
    width if necessary. Sometimes it is more efficient to
    pre-calculate it and give it as an argument}
  \item{ref.acors}{autocorrelation of the reference. Since the
    reference is often unchanged over multiple evaluations (e.g., during
    an optimization), it is useful to pre-calculate this number}
}
\details{
  All patterns in \code{samp} are warped using the same warping
  function, and then compared to \code{ref}, either pair-wise (when
  \code{ref} and \code{samp} are of the same size), or with the one
  pattern in \code{ref}.
}
\value{
One number - either the WCC or RMS value
}
\seealso{\code{\link{wcc}}}
\references{
  Eilers, P.H.C. (2004) "Parametric Time Warping", Analytical Chemistry, \bold{76} (2), 404 -- 411.

  de Gelder, R., Wehrens, R. and Hageman, J.A. (2001) "A generalized expression for the similarity of spectra: Application to powder diffraction pattern classification",  Journal of Computational Chemistry, \bold{22}, 273 -- 289.
}
\author{Jan Gerretzen, Tom Bloemberg, Ron Wehrens}
\keyword{manip}
