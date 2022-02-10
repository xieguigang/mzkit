\name{plot.ptw}
\alias{plot.ptw}
\title{Plot a ptw object}
\description{
The function plots a \code{ptw} object. It shows either the original and
warped signals, or the warping function.  
}
\usage{
\method{plot}{ptw}(x, what = c("signal", "function"),
         type = c("simultaneous", "individual"), ask = TRUE, ...)
}
\arguments{
  \item{x}{object of class 'ptw'}
  \item{what}{\code{"signal"} plots the reference, sample and warped
    sample signal(s); \code{"function"} plots the warping function as 
	warped 'time' - 'time' for the forward warping mode and as 'time' - 
	warped 'time' for the backward warping mode.}
  \item{type}{\code{"simultaneous"} plots all signals or warping
    functions in one plot; \code{"individual"} generates multiple plots}
  \item{ask}{logical, whether to ask before showing a new plot}
  \item{\dots}{further arguments to the plotting functions}
}
\author{Jan Gerretzen, Ron Wehrens, Tom Bloemberg}
\examples{
data(gaschrom)
ref <- gaschrom[1:8,]
samp <- gaschrom[9:16,]
gaschrom.ptw <- ptw(ref, samp, warp.type = "individual",
                    optim.crit = "RMS", init.coef = c(0, 1, 0))
plot(gaschrom.ptw)
plot(gaschrom.ptw, what = "function")
}
\keyword{manip}
