#Region "Microsoft.ROpen::0f5ca9243ed6f3828d004ae68b985b55, PrecursorType.R"

    # Summaries:

    # PrecursorType <- function() {...
    # adduct.mz <- function(mass, adduct, charge) {...
    # adduct.mz.general <- function(mass, adduct, charge) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # reverse.mass <- function(precursorMZ, M, charge, adduct) {...
    # reverse.mass.general <- function(precursorMZ, M, charge, adduct) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # .addKey <- function(type, charge, M, adducts) {# Evaluate the formula expression to weightsif (!is.numeric(adducts)) {...
    # positive <- function() {...
    # negative <- function() {...

#End Region

# https://github.com/xieguigang/MassSpectrum-toolkits/blob/6f4284a0d537d86c112877243d9e3b8d9d35563f/DATA/ms2_math-core/Ms1/PrecursorType.vb

#' The precursor type data model
#'
#' @details This helper function returns a list, with members:
#'    \enumerate{
#'       \item \code{mz} Calculate mass \code{m/z} value with given adduct and charge values.
#'       \item \code{mass} Calculate mass value from given \code{m/z} with given adduct and charge, etc.
#'       \item \code{new} Create a new mass and \code{m/z} calculator from given adduct info
#'    }
#'
PrecursorType <- function() {

    #' Evaluate adducts text to molecular weight.
    .eval <- Eval(MolWeight)$Eval;

	#' Calculate m/z
	#'
	#' @param mass Molecule weight
	#' @param adduct adduct mass
	#' @param charge precursor charge value
	#'
	#' @return Returns the m/z value of the precursor ion
	adduct.mz <- function(mass, adduct, charge) {
		(mass + adduct) / abs(charge);
	}
	adduct.mz.general <- function(mass, adduct, charge) {
	    # Evaluate the formula expression to weights
	    if (!is.numeric(adducts)) {
	        adducts <- .eval(adducts);
	    }

	    adduct.mz(mass, adduct, charge);
	}

	#' Calculate mass from m/z
	#'
	#' @description Calculate the molecule mass from precursor adduct ion m/z
	#'
	#' @param precursorMZ MS/MS precursor adduct ion m/z
	#' @param charge Net charge of the ion
	#' @param adduct Adduct mass
	#' @param M The number of the molecule for formula a precursor adduct ion.
	#'
	#' @return The molecule mass.
	reverse.mass <- function(precursorMZ, M, charge, adduct) {
		(precursorMZ * abs(charge) - adduct) / M;
	}
	reverse.mass.general <- function(precursorMZ, M, charge, adduct) {
	    # Evaluate the formula expression to weights
	    if (!is.numeric(adducts)) {
	        adducts <- .eval(adducts);
	    }

	    reverse.mass(precursorMZ, M, charge, adduct);
	}

	#' Construct a \code{precursor_type} model
	#'
	#' @param charge The ion charge value, no sign required.
	#' @param type Full name of the precursor type
	#' @param M The number of the target molecule
	#' @param adducts The precursor adducts formula expression
	#'
	.addKey <- function(type, charge, M, adducts) {
		# Evaluate the formula expression to weights
		if (!is.numeric(adducts)) {
			adducts <- .eval(adducts);
		}

		list(Name   = type,
			 calc   = function(precursorMZ) reverse.mass(precursorMZ, M, charge, adducts),
			 charge = charge,
			 M      = M,
			 adduct = adducts,
			 cal.mz = function(mass) adduct.mz(mass * M, adducts, charge)
		);
	}

	list(mz   = adduct.mz.general,
		 mass = reverse.mass.general,
		 new  = .addKey
	);
}