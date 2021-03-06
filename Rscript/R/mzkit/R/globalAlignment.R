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
    query = query[, "into"];
    ref   = ref[, "into"];

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

#' SSM score with mass weighted
#'
#' @description The fragment its m/z value is smaller,
#'     then weight value of this fragment is higher.
#'
#' @details The \code{query} and \code{ref} spectra matrix should be aligned by
#'    \code{\link{globalAlign}} function at first.
#'
weighted_MScos = function(query, ref) {
    # reorder the matrix row by m/z mass
    qmz   = query[, 1] %=>% as.vector;
    smz   =   ref[, 1] %=>% as.vector;
    # small m/z first
    query = rbind(query[order(qmz), ], NULL);
    ref   = rbind(ref[order(smz), ], NULL);
    # smaller the m/z value, greater weight it have
    n     = length(qmz) - (1:length(qmz)) + 1;
    # assign m/z mass weight
    query = query[, 2] * n;
    ref   = ref[, 2] * n;

    MScos(query, ref);
} 

#' Align \code{x} by using \code{y} as base matrix
#' 
globalAlign = function(x, y, tolerance = mzkit::tolerance(0.3, "da")) {

}

#'
#' @details the MS matrix of \code{query} and \code{ref} should 
#'     have been both pre-processed.
#'
#' @param query the MS matrix should be processed by \code{centroid}
#' 
MSjaccard = function(query, ref, tolerance = mzkit::tolerance(0.3, "da"), topn = 5) {
    # we have two m/z vector
    query     = MStop(query, topn);
    ref       = MStop(ref, topn);
    union     = length(numeric.group(append(query, ref), assert = tolerance$assert);
    intersect = sapply(query, function(mz) {
        sum(tolerance(mz, ref)) > 0;
    });

    sum(intersect) / union;
}

#' Take \code{m/z} by top n intensity
#' 
#' @param x a MS matrix, and it should be pre-processed by \code{centroid}.
#' 
MStop = function(x, topn = 5) {
    x = x[order(x[, 2], decreasing = TRUE), ];
    x = x[, 1];

    x[min(length(x), topn)];
}