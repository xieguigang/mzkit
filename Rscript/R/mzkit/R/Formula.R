#Region "Microsoft.ROpen::5937bfe31fd8262f3dac0bdbcb978586, R\Formula.R"

    # Summaries:

    # Enumerate.Formula.mass <- function(mass, charges = NULL, tolerance = function(m1, m2) abs(m1 - m2) <= 0.3) {}Enumerate.Formula.MZ <- function(mz, tolerance = function(mz1, mz2) ppm(mz1 - mz2) <= 20) {...

#End Region

# Hello, world!
#
# This is an example function named 'hello' 
# which prints 'Hello, world!'.
#
# You can learn more about package authoring with RStudio at:
#
#   http://r-pkgs.had.co.nz/
#
# Some useful keyboard shortcuts for package authoring:
#
#   Build and Reload Package:  'Ctrl + Shift + B'
#   Check Package:             'Ctrl + Shift + E'
#   Test Package:              'Ctrl + Shift + T'

Enumerate.Formula.mass <- function(mass, charges = NULL, tolerance = function(m1, m2) abs(m1 - m2) <= 0.3) {

}

Enumerate.Formula.MZ <- function(mz, tolerance = function(mz1, mz2) ppm(mz1 - mz2) <= 20) {

}

#' Parse formula string
#' 
#' @return a key-value pair list of \code{atom} -> \code{count}
#' 
parseFormula = function(formula) {
	chars    = Strings.Split(formula, "");
	elements = list();
	buffer   = c();
	buf_int  = c();
	isLower  = function(x) utf8ToInt(x) >= utf8ToInt("a") && utf8ToInt(x) <= utf8ToInt("z");
	isUpper  = function(x) utf8ToInt(x) >= utf8ToInt("A") && utf8ToInt(x) <= utf8ToInt("Z");
	isInt    = function(x) utf8ToInt(x) >= utf8ToInt("0") && utf8ToInt(x) <= utf8ToInt("9");
	work     = environment();
	push     = function() {
		atom = paste(buffer, collapse = "");
				
		if (is.null(buf_int)) {
			n = 1;
		} else {
			n = paste(buf_int, collapse = "");
			n = as.numeric(n);
		}
						
		assign("buffer", NULL, envir = work);
		assign("buf_int", NULL, envir = work);
		
		if (atom %in% names(elements)) {
			elements[[atom]] = elements[[atom]] + n;
		} else {
			elements[[atom]] = n;
		}
		
		assign("elements", elements, envir = work);
	}
	
	for(c in chars) {
		if (isUpper(c)) {
			if (!is.null(buffer)) {
				push();
			}
			
			buffer = append(buffer, c);
		} else if (isLower(c)) {
			buffer = append(buffer, c);
		} else if (isInt(c)) {
			buf_int = append(buf_int, c);
		}
	}
	
	if (!IsNothing(buffer)) {
		push();
	}
	
	elements;
}