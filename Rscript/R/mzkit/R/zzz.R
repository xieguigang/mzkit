#Region "Microsoft.ROpen::7f0e2e1434852e450c6e53aeafca5a61, R\zzz.R"

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

	# class dependency should be loaded 
	# before its customer.
	.class_mzInto();
    .class_peakMs2();
    .class_atom();
    .class_tolerance();
    .class_precursor();

    check = c(
        exists("Calculator", global) && bindingIsLocked("Calculator", global),
        exists("MolWeight", global) && bindingIsLocked("MolWeight", global)
    );

	try({
        if (!any(check)) {
            list(
                #' The molweight module is the very basic function 
                #' for other modules
                MolWeight     = MolWeight(),
                PrecursorType = PrecursorType(),
                #' Get precursor ion calculator
                Calculator    = list("+" = positive(), "-" = negative())
            ) %=>% Set;

            lockBinding(sym = "Calculator", env = global);
            lockBinding(sym = "MolWeight",  env = global);
        }
	});
}

.flashLoad <- function() .onLoad(NULL, NULL);
