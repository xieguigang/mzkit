#Region "Microsoft.ROpen::ff7c41157d59876a96d4859198aa5d8e, R\globalAlignment.R"

# Summaries:


#End Region

# global alignment of two MS spectrum

#' Evaluate the Entropy difference between two spectrum
#' 
#' @details Spectral entropy outperforms MS/MS dot
#'  product similarity for small-molecule compound
#'  identification.
#' 
#' @param query a spectrum for query the database in \code{mzInto} class.
#' @param ref a reference spectrum load from the database in \code{mzInto} class.
#' 
#' @return Entropy difference score between two spectrum
#' 
MSDiffEntropy = function(query, ref) {
  SAB = MSEntropy(MixMS(query, ref));
  SA  = MSEntropy(query);
  SB  = MSEntropy(ref);
 
  # Unweighted entropy similarity
  1 - (2 * SAB - SA - SB) / log(4);
}

#' Mix two MS matrix
#' 
#' @param x a spectrum in \code{mzInto} class.
#' @param y a spectrum in \code{mzInto} class.
#' 
#' @return a spectrum in \code{mzInto} class, which is the union
#'   result of two MS matrix \code{x} and \code{y}.
#' 
MixMS = function(x, y, mzdiff = mzkit::tolerance(0.3, "da")) {
  # assume than x,y is centroid MS
  mz = append(x@mz, y@mz);
  mz = numeric.group(mz, assert = function(x, y) mzdiff$assert(x, y)) %=>%
     names %=>% 
     as.numeric;
  
  ix = .alignIntensity(x@mz, x@intensity, mz, mzdiff); 
  iy = .alignIntensity(y@mz, y@intensity, mz, mzdiff);

  # 1:1 mix
  # Virtual spectrum AB
  new("mzInto", 
    mz = mz, 
    intensity = ix / max(ix) + iy / max(iy)
  );
}

#' Align of intensity vector
#' 
#' @param mz the \code{m/z} vector
#' @param intensity the intensity vector which is corresponding to the 
#'     \code{mz} vector.
#' @param template the reference \code{m/z} vector.
#' 
#' @return an intensity vector which is aligned to the \code{template}
#'    \code{m/z} vector.
#' 
.alignIntensity = function(mz, intensity, template, mzdiff = mzkit::tolerance(0.3, "da")) {
  sapply(template, function(mzi) {
     i = mzdiff$assert(mz, mzi);

     if (sum(i) == 0) {
       0;
     } else {
       max(intensity[i]);
     }
  })
}

#' Evaluate the Entropy of a given MS object.
#' 
#' @param x a object with \code{mzInto} class.
#' 
MSEntropy = function(x) {
   p = x@intensity / max(x@intensity);
   p = -sum(p * log(p));
   p;
}

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
#' @param x the target query MS matrix data.
#' @param y the reference MS matrix data.
#' @param tolerance the mzdiff tolerance between the \code{m/z} values.
#' 
#' @return a aligned MS matrix which its \code{m/z} dimension size
#'   is equals to the size of \code{y} refernece.
#'
globalAlign = function(x, y, tolerance = mzkit::tolerance(0.3, "da")) {
  ref   = y[, 1];
  query = x[, 1];
  ints  = x[, 2];
  into  = .alignIntensity(query, ints, ref, mzdiff = tolerance); 

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
#' @return a jaccard similarity score, value range \code{[0, 1]}.
#'
MSjaccard = function(query, ref,
                     tolerance = mzkit::tolerance(0.3, "da"),
                     topn      = 5) {

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
  x = rbind(NULL, x[x[, 2] > 0, ]);
  x = x[order(x[, 2], decreasing = TRUE), ];
  x = x[, 1];

  x[1:min(length(x), topn)];
}
