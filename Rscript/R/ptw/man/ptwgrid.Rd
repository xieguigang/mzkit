\name{ptwgrid}
\alias{ptwgrid}
\title{Calculate RMS or WCC values on a grid}
\description{Calculates values of the chosen optimization criterion (RMS
  or WCC) on a grid defined by the coefficients of the warping
  function.}
\usage{
ptwgrid(ref, samp, selected.traces,
	coef.mins, coef.maxs, coef.lengths, 
	optim.crit = c("WCC", "RMS"), 
	smooth.param = 1e05, 
	trwdth = 20)
}
\arguments{
  \item{ref}{reference. Either a vector (containing one reference
    signal) or a matrix (one reference per row). If
    more than one reference is specified, the number of reference
    signals must equal the number of sample signals} 
  \item{samp}{sample. A vector (containing one sample signal) or a
    matrix (one sample per row). If
    more than one sample is specified, the number of sample
    signals must equal the number of reference signals} 
  \item{selected.traces}{optional vector containing the row numbers to
    use from \code{ref} (if more than one reference signal is specified)
    and \code{samp}}
  \item{coef.mins}{a vector containing the respective minimal values 
    of coefficients for the grid. The first number is the minimal 
    zeroth-order coefficient (i.e., a constant shift); further numbers 
    indicate the minimal linear, quadratic, ... stretches }  
  \item{coef.maxs}{a vector containing the maximal values of 
    coefficients for the grid}  
  \item{coef.lengths}{a vector of the same length as coef.maxs and 
    coef.mins containing the number of steps in which to vary the 
    respective coefficients between their minimum and maximum value}
  \item{optim.crit}{either \code{"WCC"} or \code{"RMS"}. In both cases,
    the optimal value of the alignment leads to a value of 0. For "WCC",
    this means that \code{1 - WCC} is optimized rather than \code{WCC}
    (where the optimal value equals 1)}
  \item{smooth.param}{smoothing parameter for smoothing the reference
    and sample when \code{optim.crit} equals \code{"RMS"}. If no
    smoothing is required, set this to 0}.
  \item{trwdth}{the width of the triangle in the WCC criterion during
    the optimization, given as a number of data points. Default: 20}   
}
\details{In contrast to ptw, only the three situations leading to one 
  warping function are distinguished here:
    \enumerate{
    \item One sample and one reference;
    \item Several samples, warped to an equal number of references
    (pair-wise);
    \item Several samples, warped to a single reference.
    }
    Which situation is applicable is determined from the dimensions of 
    \code{ref} and \code{samp}.
}
\value{An array of dimension \code{length(coef.mins)} and maximal indices 
  per dimension specified by \code{coef.lengths}}

\seealso{
  \code{\link{ptw}}
}
\author{Tom Bloemberg, Jan Gerretzen, Ron Wehrens}
\examples{
  \dontrun{
  data(gaschrom)
  mygrid <- ptwgrid(gaschrom[1,], gaschrom[16,],
                    coef.mins = c(-10, .9), coef.max = c(10, 1.1),
                    coef.lengths = c(21, 21))
  image(seq(-10, 10, length = 21),
        seq(.9, 1.1, length = 21),
        mygrid)
}
}
\keyword{manip}
