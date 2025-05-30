% Generated by roxygen2: do not edit by hand
% Please edit documentation in R/precursor_type.R
\name{PrecursorType}
\alias{PrecursorType}
\title{The precursor type data model}
\usage{
PrecursorType()
}
\arguments{
\item{mass}{Molecule weight}

\item{precursorMZ}{MS/MS precursor adduct ion m/z}

\item{adduct}{Adduct mass}

\item{charge}{The ion charge value, no sign required.}

\item{type}{Full name of the precursor type}

\item{M}{The number of the target molecule}

\item{adducts}{The precursor adducts formula expression}
}
\value{
Returns the m/z value of the precursor ion
Calculate mass from m/z

The molecule mass.
Construct a \code{precursor_type} model
}
\description{
Calculate the molecule mass from precursor adduct ion m/z
}
\details{
This helper function returns a list, with members:
   \enumerate{
      \item \code{mz} Calculate mass \code{m/z} value with 
             given adduct and charge values.
      \item \code{mass} Calculate mass value from given 
            \code{m/z} with given adduct and charge, etc.
      \item \code{new} Create a new mass and \code{m/z} 
             calculator from given adduct info
   }

Evaluate adducts text to molecular weight.
Calculate m/z
}
