#' Run molecule networking
#' 
#' @param peak_ms2 a small bundle of MS matrix. MS matrix items
#' in this collection should be pre-processed by \code{centroid}
#' 
MScluster = function(peak_ms2, tolerance = mzkit::tolerance(0.3, "da")) {
    cos = lapply(peak_ms2, function(ref) alignVector(ref, peak_ms2, tolerance));
    
}

alignVector = function(ref, peak_ms2, tolerance = mzkit::tolerance(0.3, "da")) {
    as.vector(sapply(peak_ms2, function(query) {
        query2  = globalAlign(query, ref, tolerance);
        forward = MScos(query2, ref);
        ref     = globalAlign(ref, query, tolerance);
        reverse = MScos(query, ref);

        min(forward, reverse);
    }));
}