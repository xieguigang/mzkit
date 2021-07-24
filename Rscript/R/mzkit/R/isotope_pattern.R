
#' Evaluate isotope pattern
#' 
#' @param formula the target formula string to evaluate
#' 
isotope_pattern = function(formula) {
    if (class(formula) == "character") {
        formula = parseFormula(formula);
    }

    
}