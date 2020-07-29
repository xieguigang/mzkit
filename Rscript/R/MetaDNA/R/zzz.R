#Region "Microsoft.ROpen::0f45f7fe6c882771617e79125cc4f0ee, zzz.R"

    # Summaries:

    # cluster.cores <- function(cores = NULL) {...
    # .onLoad <- function(libname, pkgname) {...

#End Region

#' Get/Set cluster cores for metaDNA iteration
#'
cluster.cores <- function(cores = NULL) {
    anonymous <- "metaDNA_cluster.cores_anonymous.holder";

    if (!cores %=>% IsNothing) {
        do.call(`=`, list(anonymous, cores), envir = .GlobalEnv);
    }

    get(anonymous, envir = .GlobalEnv);
}

#' Package loader
#'
.onLoad <- function(libname, pkgname) {

    require(VisualBasic.R);
    require(mzkit);

    global <- globalenv();

    # enable additional language feature
    Imports("Microsoft.VisualBasic.Data", frame = global);
    Imports("Microsoft.VisualBasic.Data.Linq", frame = global);

	try({
        # Run metaDNA parallel in full power
        cluster.cores(VisualBasic.R::getClusterCores(level = "full"));
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

    cat("\n");
    cat(sprintf("Run metaDNA iteration use %s folk process", cluster.cores()));
    cat("\n\n");

    cat("You can acquire the toolkit's source code from github:");
    cat("\n\n");
    cat("     https://github.com/xieguigang/mzkit");
    cat("\n\n");
    cat("If any problem for this package, contact the author:");
    cat("\n\n");
    cat("     xieguigang <xie.guigang@gcmodeller.org>")
    cat("\n\n");

}

.flashLoad <- function() .onLoad(NULL, NULL);
