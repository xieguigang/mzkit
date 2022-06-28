\name{select.traces}
\alias{select.traces}
\title{
  Select traces from a data set according to several criteria
}
\description{
  For alignment purposes, it may be useful to select traces which show
  clear features, and to throw away traces that contain mainly
  noise. This function implements three ways to achieve this: CODA, a
  criterion similar to varimax, and a criterion based on the highest
  intensity.
}
\usage{
select.traces(X, criterion = c("coda", "var", "int"),
              window = 5, smoothing = c("median", "mean"))
}
\arguments{
  \item{X}{a data matrix or an array. The first dimension signifies the
    traces from which a selection is to be made. If X is a matrix, the
    first usually corresponds to samples and the second dimension
    is the spectral dimension. If X is an array, the data are
    assumed to come from a hyphenated experiment, with the first
    dimension the chromatographic dimension, the second the spectral
    dimension and the third dimension corresponding to samples}
  \item{criterion}{either Windig's CODA algorithm, a criterion
    calculating the variances of the length-scaled spectra, or a
    criterion giving the height of the highest peak}
  \item{window, smoothing}{arguments to the coda function.}
}
\value{
  The function returns a list with components
  \item{crit.val}{a vector containing the values of the criterion for
    all traces. If X is an array, the function is recursively applied
    to all samples (elements of the third dimension) - the results are
    multiplied to obtain one criterion value per trace}
  \item{trace.nrs}{the order of the traces (from large to small)}
}
\details{The CODA criterion in essence selects traces with no baseline
  and no spikes, but still containing significant intensities. The
  variance criterion aims at something similar: it calculates the
  variance (or standard deviation) of every trace after length scaling -
  traces with a high value show few highly structured features, whereas
  traces with a low value show noise or a significant baseline. The
  intensity criterion simply returns the intensity of the highest
  peak. The latter two criteria are simpler than CODA but implicitly
  assume that the traces have been preprocessed (i.c., spikes have been
  removed).
}
\author{Ron Wehrens}
\seealso{\code{\link{coda}}}
\examples{
data(lcms)
ntrace <- dim(lcms)[1]
lcms.selection <- select.traces(lcms[,,1:2], criterion = "var")
good <- lcms.selection$trace.nrs[1]
bad <- lcms.selection$trace.nrs[ntrace]

par(mfrow = c(1,2))
matplot(lcms[good,,1:2], type = 'l', lty = 1)
matplot(lcms[bad,,1:2], type = 'l', lty = 1)
}
\keyword{manip}
