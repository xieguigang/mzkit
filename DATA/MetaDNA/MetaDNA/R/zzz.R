.onLoad <- function(libname, pkgname) {

    require(VisualBasic.R);

    # enable additional language feature
    Imports("Microsoft.VisualBasic.Data", frame = globalenv());
    Imports("Microsoft.VisualBasic.Data.Linq", frame = globalenv());

	try({
		list(Calculator = init_calc()) %=>% Set;
		lockBinding(sym = "Calculator", env = globalenv());
	});
    
    print("Pre-defined m/z calculator:");
    cat("\n");
    cat(" [m/z]+\n\n");
    print(sapply(Calculator$`+`, function(type) type$Name) %=>% as.character);
    cat("\n");
    cat(" [m/z]-\n\n");
    print(sapply(Calculator$`-`, function(type) type$Name) %=>% as.character);
    cat("\n");

    cat("You can acquire the toolkit's source code from github:");
    cat("\n\n");
    cat("     https://github.com/xieguigang/MassSpectrum-toolkits");
    cat("\n\n");
    cat("If any problem for this package, contact the author:");
    cat("\n\n");
    cat("     xieguigang <xie.guigang@gcmodeller.org>")
    cat("\n\n");

}

.flashLoad <- function() .onLoad(NULL, NULL);

