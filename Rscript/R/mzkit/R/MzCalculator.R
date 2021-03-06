#Region "Microsoft.ROpen::9f051c57bb12d59a48673e1e1cdec2ac, R\MzCalculator.R"

    # Summaries:

    # mz.calculator <- function(mass, mode = c(1, -1), debug = FALSE) {...

#End Region

#' Calculate m/z matrix
#'
#' @description Calculate all of the m/z value by enumerate all of the precursor types.
#'
#' @param mode The ionization mode, by default is positive mode.
#'             \enumerate{
#'                \item  1: \code{positive}
#'                \item -1: \code{negative}
#'             }.
#' @param mass The molecule weight.
#' @param debug A logical flag to indicate that this function should
#'              output debug info onto the console screen?
#'
#' @return \code{m/z} dataframe for all precursor types.
#'
mz.calculator <- function(mass, mode = c(1, -1), debug = FALSE) {
    calc <- NA;

    if (mode[1] == 1) {
        calc <- positive();
    } else {
        calc <- negative();
    }

    # Enumerate all of the precursor type in the calculator,
    # and then returns the m/z result data frame
    out <- c();

    if (debug) {
        print(names(calc));
    }

    for (name in names(calc)) {
        type <- calc[[name]];
        cal  <- type$cal.mz;

        if (debug) {
            print(type);
            print(cal);
        }

        r <- c(type$Name,
               type$charge,
               type$M,
               type$adduct,
               cal(mass)
        );
        out <- rbind(out, r);
    }

    # change data type of each column from character mode
    out <- data.frame(
      "precursor_type" = out[, 1] %=>% as.character,
      "charge"         = out[, 2] %=>% as.numeric,
      "M"              = out[, 3] %=>% as.numeric,
      "adduct"         = out[, 4] %=>% as.numeric,
      "m/z"            = out[, 5] %=>% as.numeric
    );

    rownames(out) <- names(calc);
    colnames(out) <- c("precursor_type", "charge", "M", "adduct", "m/z");

    out;
}
