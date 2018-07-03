.onLoad <- function(libname, pkgname) {
	require(VisualBasic.R);

  Imports("Microsoft.VisualBasic.Data", frame = globalenv());
  Imports("Microsoft.VisualBasic.Data.Linq", frame = globalenv());

  list(Calculator = init_calc()) %=>% Set;
  lockBinding(sym = "Calculator", env = globalenv());

  print("Pre-defined m/z calculator:");
  cat("\n");
  cat("m/z+\n\n");
  print(sapply(Calculator$`+`, function(type) type$Name) %=>% as.character);
  cat("\n");
  cat("m/z-\n\n");
  print(sapply(Calculator$`-`, function(type) type$Name) %=>% as.character);
  cat("\n");
}

.flashLoad <- function() .onLoad(NULL, NULL);

