#Region "Microsoft.ROpen::ce830ddd7d84ada8fa3cd06162456015, match.R"

    # Summaries:

    # PrecursorType.Match <- function(mass, precursorMZ, charge,chargeMode= "+",tolerance = tolerance.deltaMass(0.3),debug.echo= TRUE) { if (tolerance %=>% is.numeric) {...
    # .PrecursorType.MatchImpl <- function(mass, precursorMZ, charge,chargeMode, tolerance,debug.echo) {...
    # match <- function(keyName) {...

#End Region

#' Match the precursor type
#'
#' @description Match the precursor type through min ppm value match.
#'
#' @param charge The charge value of the ion
#' @param mass Molecular mass
#' @param precursorMZ Precursor m/z value of the ion.
#' @param tolerance Tolerance between two mass value, by default is 0.3 da,
#'    if this parameter is a numeric value, then means tolerance by ppm value.
#'    There are two pre-defined tolerance function:
#'
#'    \enumerate{
#'        \item \code{\link{tolerance.deltaMass}}
#'        \item \code{\link{tolerance.ppm}}
#'    }
#'
#' @examples mass = 853.33089
#'
#' PrecursorType.Match(853.33089, 307.432848,  charge = 3) # pos "[M+3Na]3+" charge = 3,  307.432848
#' PrecursorType.Match(853.33089, 1745.624938, charge = 1) # pos "[2M+K]+"   charge = 1,  1745.624938
#' PrecursorType.Match(853.33089, 854.338166,  charge = 1) # pos "[M+H]+"    charge = 1,  854.338166
#'
#' PrecursorType.Match(853.33089, 283.436354,  charge = -3, chargeMode = "-") # neg "[M-3H]3-"  charge = -3, 283.436354
#' PrecursorType.Match(853.33089, 2560.999946, charge = -1, chargeMode = "-") # neg "[3M-H]-"   charge = -1, 2560.999946
#' PrecursorType.Match(853.33089, 852.323614,  charge = -1, chargeMode = "-") # neg "[M-H]-"    charge = -1, 852.323614
#'
PrecursorType.Match <- function(
    mass, precursorMZ, charge,
    chargeMode   = "+",
    tolerance    = tolerance.deltaMass(0.3),
    debug.echo   = TRUE) {

    if (tolerance %=>% is.numeric) {
        tolerance <- tolerance.ppm(tolerance);
    }

    if (charge == 0) {
        warning("Can't calculate the ionization mode for no charge(charge = 0)!");
        NA;
    } else if ((mass %=>% IsNothing) || (precursorMZ %=>% IsNothing)) {
        if(is.null(mass)) {
            mass = NA;
        }
        if(is.null(precursorMZ)) {
            precursorMZ = NA;
        }

        msg <- "  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!";
        msg <- sprintf(msg, mass, precursorMZ);
        warning(msg);

        NA;
    } else if (tolerance(precursorMZ, mass / abs(charge))$valid) {
        # The source mass is equals to the precursor_m/z,
        # means it is a [M] type (auto-ionlization)
        if(abs(charge) == 1) {
            sprintf("[M]%s", chargeMode);
        } else {
            sprintf("[M]%s%s", charge, chargeMode);
        }
    } else {
        .PrecursorType.MatchImpl(
            mass, precursorMZ, charge,
            chargeMode, tolerance,
            debug.echo
        );
    }
}

#' Calculate for each precursor type, and then returns the
#' min tolerance type as the match result
.PrecursorType.MatchImpl <- function(
    mass, precursorMZ, charge,
    chargeMode, tolerance,
    debug.echo) {

    ## Get the calculator in current ion mode
    mode <- Calculator[[chargeMode]];

    if (chargeMode == "-") {
        ## For the negative mode, the charge is a negative value.
        ## But the xcms package extract a positive charge value.
        ## So this required a negative factor
        if (charge > 0) {
            charge = -1 * charge;
        }
    }

    match <- function(keyName) {
        calc <- mode[[keyName]];

        ## Skip the precursor type where the charge value is not match
        if (charge != calc$charge) {
            note  <- "charge mismatched!";
            error <- NA;
            valid <- FALSE
        } else {
            # Calculate the mass from precursor mz and then calculate the
            # mass tolerance
            mass.reverse <- calc$calc(precursorMZ);
            validate     <- tolerance(mass.reverse, mass);

            if (validate$valid) {
                note <- NA;
            } else {
                note <- "Tolerance not satisfied!";
            }

            error <- validate$error;
            valid <- validate$valid;
        }

        calc$error  <- error;
        calc$valid  <- valid;
        calc$calc   <- NULL;
        calc$cal.mz <- NULL;
        calc$note   <- note;

        calc;
    }

    ## enumerate all of the types in current mode
    match <- lapply(mode %=>% names, match);
    match <- match %=>% as.dataframe;

    # > head(match)
    #   Name          charge M adduct   error valid
    # 1 "[M+3H]3+"    3      1 3.021828 NA    FALSE
    # 2 "[M+2H+Na]3+" 3      1 25.00432 NA    FALSE
    # 3 "[M+H+2Na]3+" 3      1 46.98681 NA    FALSE
    # 4 "[M+3Na]3+"   3      1 68.96931 NA    FALSE
    # 5 "[M+2H]2+"    2      1 2.014552 NA    FALSE
    # 6 "[M+H+NH4]2+" 2      1 19.04281 NA    FALSE
    if (debug.echo) {
        print(match);
    }

    # find which result its tolerance is valid
    valids <- (match[, "valid"] == TRUE) %=>% as.logical %=>% which;

    if (length(valids) == 0) {
        # no result
        min <- NA;
    } else if (length(valids) > 1) {
        # get the min tolerance result
        # if exists multiple valid result, then it means all of their
        # error is under the cutoff
        # So get the min error one as the result.
        min <- match[, "error"] %=>% as.numeric %=>% which.min;
    } else {
        # get the unique result
        min <- valids;
    }

    if (min %=>% IsNothing) {
        msg <- "No precursor_type: [mass=%s, m/z=%s] charge=%s(%s) with tolerance=%s";
        msg <- sprintf(msg,
                       mass, precursorMZ,
                       charge, chargeMode,
                       tolerance(0, 0)$describ
        );
        warning(msg);
        NA;
    } else {
        min <- match[min, ];

        if (debug.echo) {
            printf("  ==> %s\n", min[["Name"]]);
        }

        # we found it!
        min[["Name"]];
    }
}
