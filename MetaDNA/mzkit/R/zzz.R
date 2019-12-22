
#' Package loader
#'
.onLoad <- function(libname, pkgname) {
    require(VisualBasic.R);

	global <- globalenv();

    # enable additional language feature
    Imports("Microsoft.VisualBasic.Data", frame = global);
    Imports("Microsoft.VisualBasic.Data.Linq", frame = global);
}

.flashLoad <- function() .onLoad(NULL, NULL);