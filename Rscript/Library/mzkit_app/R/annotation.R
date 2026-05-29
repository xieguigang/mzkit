imports ["metadb", "massbank", "math"] from "mzkit";
imports "metadna" from "mzDIA";

#' Load the kegg database for the annotation
#' 
#' @param precursors a character vector of the precursor adducts type, 
#'     apply for the precursor m/z evaluation.
#' @param mzdiff a tolerance error value for make ms1 matches
#' @param repofile the local database file for the kegg compounds. should be a 
#'     messagepack data file of a collection of the GCModeller compound object.
#' @param strict option strict for check of the file exists or not of the repo file. 
#'     an error was throw if the given repo file is missing from the local filesystem, 
#'     or use the kegg compound data repo file from GCModeller internal resource file
#'     if not strict and also the given repo file is invalid
#' 
#' @return a wrapper of the mass search engine which have kegg compound library loaded.
#' 
const kegg_compounds = function(precursors = ["[M]+", "[M+H]+", "[M+H-H2O]+"], 
                                mzdiff     = "ppm:20", 
                                repofile   = "KEGG_compounds.msgpack", 
                                strict     = FALSE) {

    if (is.null(repofile) || !file.exists(repofile)) {
        if (strict) {
            stop(`no repository data file at location: ${repofile}!`);
        } else {
            repofile = NULL;
            warning(`the given kegg compound repository file '${repofile}' could not be found, use the default kegg compound data repository from GCModeller package.`);
        }        
    } 

    let keggSet = {
        if (is.null(repofile)) {
            GCModeller::kegg_compounds(rawList = TRUE);
        } else {
            # read messagepack repository data file
            kegg.library(repofile);
        }
    }

    keggSet |> annotationSet(
        precursors = precursors, 
        mzdiff = mzdiff
    );
}