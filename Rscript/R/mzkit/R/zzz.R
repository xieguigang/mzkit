#Region "Microsoft.ROpen::0939b08fe05398792e44db93126624c4, zzz.R"

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
        lockBinding(sym = "MolWeight",  env = global);
	});
}

.flashLoad <- function() .onLoad(NULL, NULL);
