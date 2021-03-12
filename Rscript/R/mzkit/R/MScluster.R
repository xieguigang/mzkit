#Region "Microsoft.ROpen::e227efceb1f8ee6b807bcded07a90121, R\MScluster.R"

    # Summaries:


#End Region

#' Run molecule networking
#' 
#' @param peak_ms2 a small bundle of MS matrix. MS matrix items
#' in this collection should be pre-processed by \code{centroid}
#' 
MScluster = function(peak_ms2, identical = 0.85, greaterThan = 0.6, tolerance = mzkit::tolerance(0.3, "da")) {
    cos = function(query, ref) {
        query2  = globalAlign(query, ref, tolerance);
        forward = MScos(query2, ref);
        ref     = globalAlign(ref, query, tolerance);
        reverse = MScos(query, ref);

        # gets the final global alignment score
        alignScore = min(forward, reverse);

        if (alignScore >= identical) {
            0;
        } else if (alignScore >= greaterThan) {
            1;
        } else {
            -1;
        }
    }

    # returns MS tree clusters
    bclusterTree::bcluster(peak_ms2, compares = cos);
}

BinId = function(clusterList, peak_ms2) {
	
}