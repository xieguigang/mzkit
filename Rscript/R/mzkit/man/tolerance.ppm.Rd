% Generated by roxygen2: do not edit by hand
% Please edit documentation in R/tolerance.R
\name{tolerance.ppm}
\alias{tolerance.ppm}
\title{Tolerance in PPM mode}
\usage{
tolerance.ppm(ppm = 20)
}
\arguments{
\item{ppm}{The mass ppm value. By default if two mass value which
their ppm delta value is less than \code{20ppm}, then
the predicate will be true.}
}
\value{
Function returns a lambda function that can be using for
        tolerance predication.
}
\description{
Tolerance in PPM mode
}
