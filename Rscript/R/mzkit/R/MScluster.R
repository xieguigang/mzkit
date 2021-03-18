#Region "Microsoft.ROpen::e227efceb1f8ee6b807bcded07a90121, R\MScluster.R"

    # Summaries:


#End Region

#' Run molecule networking
#' 
#' @param peak_ms2 a small bundle of MS matrix. MS matrix items
#' in this collection should be pre-processed by \code{centroid}
#' 
MScluster = function(peak_ms2, 
					 identical     = 0.85, 
					 greaterThan   = 0.6, 
					 tolerance     = mzkit::tolerance(0.3, "da"), 
					 show_progress = TRUE) {
					 
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
    bclusterTree::bcluster(peak_ms2, compares = cos, show_progress = show_progress);
}

#' create a unify BIN id of MS data
#'
BinId = function(clusterList, peak_ms2) {
	mzErr = mzkit::tolerance(0.3, "da");
	topMz = function(mz) {
		if (all(sapply(mz, IsNothing))) {
			NULL;
		} else {
			mz = mz[!sapply(mz, IsNothing)];
			mz = numeric.group(mz, mzErr$assert);
			i  = as.vector(sapply(mz, length));
			i  = order(i, decreasing = TRUE);
			mz = as.numeric(names(mz)[i][1]);
			
			round(mz);
		}
	}
	names = sapply(clusterList, function(peakIds) {		
		top3 = lapply(peak_ms2[peakIds], function(Ms2) MStop(Ms2, topn = 3));
		
		x = as.vector(sapply(top3, function(vec) vec[1])) %=>% topMz; 
		y = as.vector(sapply(top3, function(vec) vec[2])) %=>% topMz; 
		z = as.vector(sapply(top3, function(vec) vec[3])) %=>% topMz; 
		
		vec = c(x, y, z);
		
		paste0(vec, collapse=",");
	});
	
	sprintf("BIN-%s", as.vector(names));
}