\name{padzeros}
\alias{padzeros}
\title{Pad matrix with zeros}
\description{Adds zeros to the left side of a matrix or vector, to its right side, or to both sides.}
\usage{
padzeros(data, nzeros, side="both")}
\arguments{
  \item{data}{the original matrix or vector}
  \item{nzeros}{number of columns to add on one side}
  \item{side}{to which side to add the zeros - choose between "\code{both}", "\code{left}" or "\code{right}"}
}
\value{A matrix with the same number of rows as the original matrix, and extra columns containing zeros on the specified side or sides}
\details{When \code{data} is a numeric vector, it is converted to a matrix of a single row.}

\references{
  Bloemberg, T.G., et al. (2010) "Improved parametric time warping for Proteomics", Chemometrics and Intelligent Laboratory Systems, \bold{104} (1), 65 -- 74.
}

\examples{
data(lcms)
lcms.z1 <- padzeros(lcms[75,,1], 250, side="left")
lcms.z2 <- padzeros(lcms[75,,1], 250, side="right")
lcms.z3 <- padzeros(lcms[75,,1], 250, side="both")
zeros <- rep(0, 250)

layout(matrix(1:4,2,2, byrow=TRUE))
plot(lcms[75,,1], type="l", main="Original signal")

plot(as.vector(lcms.z1), type="l", main="Padzeros left side")
points(1:250, zeros, col=2, lwd=0.08)

plot(as.vector(lcms.z2), type="l", main="Padzeros right side")
points(2001:2250, zeros, col=2, lwd=0.08)

plot(as.vector(lcms.z3), type="l", main="Padzeros both sides")
points(1:250, zeros, col=2, lwd=0.08)
points(2251:2500, zeros, col=2, lwd=0.08)
}

\author{Jan Gerretzen}
\keyword{manip}
