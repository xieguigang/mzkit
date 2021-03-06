#' Run molecule networking
#' 
#' @param peak_ms2 a small bundle of MS matrix. MS matrix items
#' in this collection should be pre-processed by \code{centroid}
#' 
MScluster = function(peak_ms2, tolerance = mzkit::tolerance(0.3, "da")) {
    cos = lapply(peak_ms2, function(ref) {
        # get distance
        1 - alignVector(ref, peak_ms2, tolerance);
    });
    cos  = t(as.data.frame(cos));
    d    = dist(cos);
    tree = hclust(d, method = 'average');

    # plot(tree, hang = -1, cex=.8, main = 'average linkage clustering');

    # returns MS tree clusters
    cutree(tree, k = 3 + floor(length(peak_ms2) / 9));
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