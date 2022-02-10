\name{coda}
\alias{coda}
\title{Chromatogram selection using the CODA algorithm}
\description{
  The CODA algorithm calculates a so-called MCQ (Mass Chromatogram
  Quality) value for every row of the input. High MCQ values correspond
  with those chromatograms not containing spikes and/or a baseline.
}
\usage{coda(x, window = 5, smoothing = c("median", "mean"))}
\arguments{
  \item{x}{data matrix containing chromatograms in the rows}
  \item{window}{width of the smoothing window}
  \item{smoothing}{type of smoothing: whether to use running means or
    running medians}
}
\details{
  The MCQ value of a spectrum is the inner product between the
  standardized, smoothed chromatogram, and the length-scaled
  chromatogram. In literature, a cut-off of 0.85 has been reported to
  work well in selecting useful chromatograms, although this is strongly
  data-set dependent.
}
\references{
  Windig, W., Phalp, J., Payna, A. (1996) "A noise and background reduction method for component detection in liquid chromatography/mass spectrometry",
Analytical Chemistry, \bold{68}, 3602 -- 3606.
}
\examples{
data(gaschrom)
coda(gaschrom)
}
\keyword{manip}
