imports ["metadb", "massbank", "math"] from "mzkit";
imports "metadna" from "mzDIA";

#' Load the kegg database for the annotation
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