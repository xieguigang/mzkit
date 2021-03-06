#Region "Microsoft.ROpen::e2262f8b5aed6992a3cac2def9674d95, R\Utils.R"

    # Summaries:

    # getPolarity <- function(type) {...
    # get.mass <- function(chargeMode, PrecursorType) {if (PrecursorType %in% c("[M]+", "[M]-")) {...
    # get.PrecursorMZ.Auto <- function(M, precursorType) {...
    # get.PrecursorMZ <- function(M, precursorType, mode) {mzVector <- function(type, mode) {...
    # notFound.warn <- function(mz, precursorType) {if ((mz == -1) %=>% all) {...

#End Region

#' Get ionlization mode
#'
#' @description Get ionlization mode from a given precursor type name
#'
#' @param type The precursor type name, it should be in format like: \code{[M+H]+}.
#'
#' @return Function returns character \code{+} or \code{-}.
getPolarity <- function(type) {
  substr.Right(type, n=1);
}

#' Get mass calculator
#'
#' @param chargeMode Character value of \code{+/-}.
#' @param PrecursorType The precursor type full name or brief name.
#'
#' @return Returns a function for calculate mass from \code{m/z} value.
#'
get.mass <- function(chargeMode, PrecursorType) {
  if (PrecursorType %in% c("[M]+", "[M]-")) {
    function(x) x;
  } else {
    mode <- Calculator[[chargeMode]];
    found <- mode[[PrecursorType]];

    if (found %=>% IsNothing) {
      # Is the precursor type full name.
      for (name in names(mode)) {
        calc <- mode[[name]];
        if (calc$Name == PrecursorType) {
          found <- calc;
          break;
        }
      }
    }

    found$calc;
  }
}

#' Calculate m/z
#'
#' @description Calculate \code{m/z} for mass by given precursor type
#'
#' @param M Molecule mass
#' @param precursorType Charge mode was parsed from this parameter string,
#'      it is convenient, but inefficient when in batch mode.
#'
#' @return -1 means target precursor type is not found.
#'
get.PrecursorMZ.Auto <- function(M, precursorType) {
  get.PrecursorMZ(M, precursorType, mode = getPolarity(precursorType));
}

#' Calculate m/z with given charge mode
#'
#' @param mode The ion charge mode, if the \code{precursorType} is multiple length,
#'       then this mode parameter should be the same length as the \code{precursorType}.
#'
#' @return If \code{precursorType} just have one element, then this function will
#'      returns a numeric vector.
#'      If this parameter \code{precursorType} contains multiple type names, then this
#'      function will returns a list object with member name is the \code{precursorType}
#'      member value is the corresponding \code{m/z} vector
get.PrecursorMZ <- function(M, precursorType, mode) {
  mzVector <- function(type, mode) {
    types <- Calculator[[mode]];
    loop <- lapply(types, function(calc) {
      if (type == calc$Name) {
        calc$cal.mz(M);
      } else {
        NULL;
      }
    });
    loop <- loop[!sapply(loop, is.null)];

    if (length(loop) == 0) {
      rep(1, length(M));
    } else {
      loop[[1]];
    }
  }
  notFound.warn <- function(mz, precursorType) {
    if ((mz == -1) %=>% all) {
      warnMsg <- "\"%s\" is not found... Precursor m/z is set to -1.";
      warnMsg <- sprintf(warnMsg, precursorType);
      warning(warnMsg);
    }

    invisible(NULL);
  }

  if (length(precursorType) == 1) {
    precursorMZ <- mzVector(precursorType, mode);
    #' test and warn
    notFound.warn(precursorMZ, precursorType);
  } else {
    precursorMZ <- lapply(1:length(precursorType), function(i) {
      mzVector(precursorType[i], mode[i]);
    });
    names(precursorMZ) <- precursorType;

    lapply(names(precursorMZ), function(type) {
      #' test and warn
      notFound.warn(precursorMZ[[type]], type);
    });
  }

  precursorMZ;
}
