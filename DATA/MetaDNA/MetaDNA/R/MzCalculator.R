#Region "Microsoft.ROpen::edd43f505309ed00d55d3a0c985d1921, MzCalculator.R"

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
mz.calculator <- function(mass, mode = c(1, -1), debug = FALSE) {
    calc <- NA;

    if (mode[1] == 1) {
        calc <- positive();
    } else {
        calc <- negative();
    }

    # 枚举计算器之中的所有的前体离子的类型，然后计算完成之后返回数据框
    out <- c();

    if (debug) {
        print(names(calc));
    }

    for (name in names(calc)) {
        type <- calc[[name]];
        cal <- type$cal.mz;

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

    rownames(out) <- names(calc);
    colnames(out) <- c("precursor_type", "charge", "M", "adduct", "m/z");
    out;
}
