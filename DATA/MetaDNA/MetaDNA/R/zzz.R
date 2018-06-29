.onLoad <- function(libname, pkgname) {
	require(VisualBasic.R);

  Imports("Microsoft.VisualBasic.Data", frame = globalenv());
  Imports("Microsoft.VisualBasic.Data.Linq", frame = globalenv());
}

.flashLoad <- function() .onLoad(NULL, NULL);

