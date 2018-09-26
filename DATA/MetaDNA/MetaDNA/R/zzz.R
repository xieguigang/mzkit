#Region "Microsoft.ROpen::0afb6e2c67fd35dd806134f829dfab57, zzz.R"

    # Summaries:

    # .onLoad <- function(libname, pkgname) {...

#End Region

#' Package loader
#'
.onLoad <- function(libname, pkgname) {

    require(VisualBasic.R);

	global <- globalenv();

    # enable additional language feature
    Imports("Microsoft.VisualBasic.Data", frame = global);
    Imports("Microsoft.VisualBasic.Data.Linq", frame = global);

	try({
		list(
		  #' The molweight module is the very basic function for other modules
		  MolWeight     = MolWeight(),
		  PrecursorType = PrecursorType(),
			#' Get precursor ion calculator
			Calculator    = list("+" = positive(), "-" = negative())
		) %=>% Set;

		lockBinding(sym = "Calculator", env = global);
	});

    print("Pre-defined m/z calculator:");
    cat("\n");
    cat(" [m/z]+\n\n");
    print(sapply(Calculator$`+`, function(type) type$Name) %=>% as.character);
    cat("\n");
    cat(" [m/z]-\n\n");
    print(sapply(Calculator$`-`, function(type) type$Name) %=>% as.character);
    cat("\n");

	  cat(" [Symbol Weights]\n\n");
	  print(data.frame(
	    symbols = names(Eval(MolWeight)$weights),
	    weights = Eval(MolWeight)$weights %=>% as.numeric
	  ));
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
