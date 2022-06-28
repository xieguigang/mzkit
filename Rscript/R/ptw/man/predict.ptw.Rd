\name{predict.ptw}
\alias{predict.ptw}
\title{Prediction of warped signals}
\description{Given a ptw object, predict either the signal at a certain
  warped time, or the warped time itself.}
\usage{
\method{predict}{ptw}(object, newdata, what = c("response", "time"),
  RTref = NULL, \dots)
}
\arguments{
  \item{object}{An object of class "ptw"}
  \item{newdata}{Optional vector or matrix of new data points. If
    \code{what} equals "response", the new data should be a vector or
    matrix of intensities. If \code{what} equals "time", the new data is
    a vector of time points (a matrix of time points makes no sense...).}
  \item{what}{Either "response", in which case the function returns the
    warped signal, or "time", and then the function returns the warped
    time axis. That is, the time point in the warped sample
    corresponding to the given time point in the original sample.}
  \item{RTref}{Optional vector of retention times in the reference.}
  \item{\dots}{Further arguments, at the moment not used.}
}
\value{
  The function returns a matrix (possibly containing only one
  row) of either warped time points or signals, warped according to the
  warping function defined in \code{object}. When warping signals
  individually, \code{predict.ptw} will check the dimension of  
  \code{newdata}: if this is a vector or a matrix of one row, every
  single warping function will be applied to the one row. If the
  number of rows equals the number of warping functions, each row will
  be warped with its corresponding function. If the number of rows does
  not match the number of warping functions and is not equal to one, an
  error is given.
}
\references{
  Eilers, P.H.C. "Parametric Time Warping." Anal. Chem., 2004, 76, 404-411

  Bloemberg, T.G. et al. "Improved parametric time warping for
  proteomics." Chemom. Intell. Lab. Syst., 2010, 104, pp. 65-74
}
\author{Ron Wehrens}
\seealso{
  \code{\link{ptw}}
}
\examples{
## educational example, contributed by zeehio (Sergio Oller)
x1 <- c(rep(0, 5), 1,1,1, 20, 40, 20, 1, 1, 1, rep(0, 5))
x2 <- c(rep(0, 6), 1,1,1, 20, 40, 20, 1, 1, 1, rep(0, 4))
time <- 1:length(x1)
## get time-warped object. Default: 'forward' warping, also works
## with backward warping
w1b <- ptw(ref = x1, samp = x2) 
## predict intensities of object x2 after warping at the times used in x1
x2wb <- predict(w1b, newdata = x2, what = "response")
## predict times where the original elements of x2 will end up
t2wb <- as.numeric(predict(w1b, newdata = time, what = "time"))

graphics.off()
par(mfrow = c(2,1))
plot(x1, type = "h", col = 2, lwd = 2, main = "Orig data")
points(x2, type = "h", col = 4)

plot(x1, type = "h", col = 2, lwd = 2, main = "Backward warping")
points(c(x2wb), type = "h", col = 4) # what = "response"
points(t2wb, x2, col = 4)            # what = "time"

## more relevant example
data(gaschrom)
## Global warping: all samples warped with the same function
ref <- gaschrom[1,]
samp <- gaschrom[14:16,]
gp <- ptw(ref, samp, init.coef = c(0, 1), warp.type = "global")
matplot(t(samp), type = "l", xlim = c(2200, 2400), lty = 1, col = 1:3)
lines(ref, type = "l", col = "gray", lwd = 2)
## plot predicted warped signal directly
matlines(t(predict(gp)), lty = 2, col = 1:3)
## plot original signal at warped time axis
matlines(t(predict(gp, newdata = 2001:2600, what = "time")),
         t(samp[,2001:2600]), col = 1:3, lwd = 3, lty = 2) ## OK
## result: good alignment with ref, differences between three profiles persist

## Individual warping: all samples warped individually
gp <- ptw(ref, samp, init.coef = c(0, 1), warp.type = "indiv")
predict(gp, what = "time", newdata = 2001:2600)
matplot(t(samp), type = "l", xlim = c(2200, 2400), lty = 1, col = 1:3)
lines(ref, type = "l", col = "gray", lwd = 2)
matlines(t(predict(gp, what = "time")),
         t(samp), col = 1:3, lty = 2)
## result: each individual profile is aligned to the ref

## How would samples 11:13 be warped using the coefficients from samples
## 14:16 (silly but just to make the point)?
samp.pred <- predict(gp, what = "response", newdata = gaschrom[11:13,])
}
\keyword{manip}
