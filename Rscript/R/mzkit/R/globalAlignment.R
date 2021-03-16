#Region "Microsoft.ROpen::ff7c41157d59876a96d4859198aa5d8e, R\globalAlignment.R"

    # Summaries:


#End Region

# global alignment of two MS spectrum

#' SSM score in one direction
#'
#' @details the MS matrix of \code{query} and \code{ref} should
#'     have been both pre-processed.
#'
#' @param query the MS matrix should be processed by \code{centroid}
#'     and \code{globalAlign}
#'
MScos = function(query, ref) {
	MScos.score(
		query = query[, "into"],
		ref   = ref[, "into"]
	);
}

MScos.score = function(query, ref) {
    if (all(query == 0) || all(ref == 0)) {
        0;
    } else {
        score = sum(query * ref) / sqrt(sum(query ^ 2) * sum(ref ^ 2));

        if (is.nan(score) || is.na(score) || score == Inf || score == -Inf) {
            0;
        } else {
            score;
        }
    }
}

weighted_into = function(MS, weights) {
	# reorder the matrix row by m/z mass
    mz = as.vector(MS[, 1]);
	# small m/z first
    MS = rbind(MS[order(mz), ], NULL);
	# assign m/z mass weight
    MS = MS[, 2] * weights;
	MS;
}

#' SSM score with mass weighted
#'
#' @description The fragment its m/z value is smaller,
#'     then weight value of this fragment is higher.
#'
#' @details The \code{query} and \code{ref} spectra matrix should be aligned by
#'    \code{\link{globalAlign}} function at first.
#'
weighted_MScos = function(query, ref) {
    # smaller the m/z value, greater weight it have
    weights = nrow(query) - (1:nrow(query)) + 1;
    x = weighted_into(query, weights);
	y = weighted_into(ref, weights);

    MScos.score(x, y);
}

#' Align \code{x} by using \code{y} as base matrix
#'
globalAlign = function(x, y, tolerance = mzkit::tolerance(0.3, "da")) {
    tolerance = tolerance$assert;
    ref   = y[, 1];
    query = x[, 1];
    ints  = x[, 2];
    into  = sapply(ref, function(mz) {
        i = which(tolerance(mz, query));

        if (length(i) == 0) {
            0;
        } else {
            max(ints[i]);
        }       
    });

    data.frame(mz = ref, into = into);
}

#' Jaccard index between two MS matrix
#'
#' @details the MS matrix of \code{query} and \code{ref} should
#'     have been both pre-processed.
#'
#' @param query the MS matrix should be processed by \code{centroid}
#' @param ref the MS matrix should be processed by \code{centroid}
#'
MSjaccard = function(query, ref, tolerance = mzkit::tolerance(0.3, "da"), topn = 5) {
    # we have two m/z vector
    query = MStop(query, topn);
    ref   = MStop(ref, topn);
	
	if (length(query) > length(ref)) {
		tmp   = query;
		query = ref;
		ref   = tmp;
	}
	
    tolerance = tolerance$assert;
    union     = numeric.group(append(query, ref), assert = tolerance) %=>% names %=>% as.numeric;
    query     = numeric.group(query, assert = tolerance) %=>% names %=>% as.numeric;
	intersect = sapply(query, function(mz) {
        sum(tolerance(mz, ref)) > 0;
    });

    sum(intersect) / length(union);
}

#' Take \code{m/z} by top n intensity
#'
#' @param x a MS matrix, and it should be pre-processed by \code{centroid}.
#'
MStop = function(x, topn = 5) {
	x = rbind(NULL, x);
    x = x[order(x[, 2], decreasing = TRUE), ];
    x = x[, 1];

    x[1:min(length(x), topn)];
}
