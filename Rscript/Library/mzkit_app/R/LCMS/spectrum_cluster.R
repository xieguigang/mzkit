imports "MoleculeNetworking" from "mzDIA";

#' Get n representive spectrums via molecular networking
#' 
#' @param ions a collection of the peakms2 dataset
#' @param top_n this function returns top n spectrum data
#' 
#' @return a collection of peakms2 data object, number of
#'    the spectrum is specificed via the ``top_n`` parameter.
#' 
const get_representives = function(ions, top_n = 5, 
                                   mzdiff = 0.3,
                                   intocutoff = 0.05,
                                   equals = 0.85) {

    const tree = MoleculeNetworking::tree(
        ions = uniqueNames(ions), 
        mzdiff = mzdiff, 
        intocutoff = intocutoff, 
        equals = equals
    );
    # collisionEnergy is tagged as the cluster size
    const union_spectrums = MoleculeNetworking::representative(
        tree, ions, 
        mzdiff = `da:${mzdiff}`
    );
    
    # and then we can order the spectrum collection via 
    # the cluster size
    ions = unlist(union_spectrums);
    
    if (!is.null(ions)) {
        ions = ions[order([ions]::collisionEnergy, decreasing = TRUE)];
        # get top n spectrum from the union 
        # spectrum result
        ions[1:top_n] |> which(x -> !is.null(x));
    } else {
        NULL;
    }
}