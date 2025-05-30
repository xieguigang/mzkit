#Region "Microsoft.ROpen::f293e29168ad61ea262637db0d5d15a8, algorithm.R"

    # Summaries:

    # metaDNA.impl <- function(unknown.query, identify.ms2,unknown,ms2.align,trace,score.cutoff = 0.8) {...
    # align_best.internal <- function(ref, peak, ms2.align, score.cutoff = 0.8, parent = NA) {# Error in `colnames<-`(`*tmp*`, value = c("ProductMz", "LibraryIntensity")) :#attempt to set 'colnames' on an object with less than two dimensionsif (is.null(nrow(ref))) {...
    # pickbest.internal <- function(align) {...

#End Region

#' Try to annotate unknown metabolite
#'
#' @description Try to annotate the unknown metabolite as a given set of
#'     kegg metabolite candidates. The ms2 alignment is based on the identified
#'     metabolite ms2 data.
#'
#' @param KEGG.partners Related to the identified KEGG id based on the kegg reaction definitions.
#'     Using for find unknown metabolite ms2 data.
#' @param identify.ms2 The identify metabolite's MS/MS matrix data.
#' @param unknown Unknown metabolite's peaktable and peak_ms2 data.
#' @param ms2.align The ms2 alignment method, this function method should returns \code{forward}
#'      and \code{reverse} alignment score result list which its data structure in format like:
#'
#'      \code{list(forward = score1, reverse = score2)}.
#'
#' @param score.cutoff SSM algorithm alignment score cutoff, by default is \code{0.8}. The unknown
#'      metabolite which its forward and reverse alignment score greater than this cutoff value both,
#'      will be picked as the candidate result.
#'
#' @details Algorithm routine:
#'
#'    \code{\cr
#'          KEGG.partners -> kegg.match.handler\cr
#'                        -> unknown index\cr
#'                        -> unknown ms2\cr
#'                        -> identify.ms2 alignment\cr
#'                        -> is similar?\cr
#'    }
#'
#'    \enumerate{
#'        \item yes, identify the unknown as \code{\link{kegg.partners}}
#'        \item no, returns \code{NULL}
#'    }
#'
#'    One identify metabolite have sevral kegg partners based on the metabolism network definition
#'    So, this function find every partner in unknown, and returns a set of unknown identify result
#'    But each unknown identify only have one best identify ms2 alignment result.
#'
#' @return A set of unknown identify result based on the given related kegg partners id set.
#'
metaDNA.impl <- function(unknown.query, identify.ms2,
                         unknown,
                         ms2.align,
                         trace,
                         score.cutoff = 0.8) {
    # unknown.query is the kegg partner matched ms feature result:
    # It contains a data structure like:
    # {
    #    $unknown.index (row index in peaktable)
    #    $unknown.mz (The ms1 m/z of the unknown feature)
    #    $precursor_type (calculate from ms1 m/z between matched KEGG partner its m/z value)
    #    $kegg (The kegg compound annotation data, contains kegg_id, exact_mass, name, etc)
    #    $ppm (ppm value between the unknown ms1 m/z and the matched KEGG partner m/z value)
    #    $libmz (The m/z value that calculated from the matched KEGG partner with given precursor_type)
    # }

    # unknown.i integer index of the peaktable
    unknown.i <- sapply(unknown.query, function(x) x$unknown.index) %=>% unlist %=>% as.numeric;
    # subset of the peaktable by using the unknown index value
    # the peaktable subset object contains ms1 feature and ms2 feature
    unknown.features <- unknown[unknown.i];

    # the identify ms2 file#scan index
    # use this index value for avoid self alignment bugs.
    parent <- trace$parent;

    # 2019-03-29 these object is in length equals
    #
    # unknown.query
    # unknown.i
    # unknown.features

    # alignment of the ms2 between the identify and unknown
    # The unknown will identified as identify.ms2 when ms2.align
    # pass the threshold cutoff.
    query.result <- lapply(1:length(unknown.features), function(i) {
        # loop on current unknown match list from the identify kegg partners
        # identify for each unknown metabolite
        kegg.query <- unknown.query[[i]];
        feature <- unknown.features[[i]];

        # peak is files and scans
        peak   <- feature$ms2;
        result <- align_best.internal(
            ref          = identify.ms2,
            peak         = peak,
            ms2.align    = ms2.align,
            score.cutoff = score.cutoff,
            parent       = parent
        );

        if (!is.null(result)) {
            # name is the peaktable rownames
            feature$ms2   <- NULL;
            # add reference spectra matrix data
            result$ref    <- identify.ms2;
            result$trace  <- trace$path;
            result$parent <- sprintf("%s#%s", trace$ref, trace$parent);

            list(
                feature   = feature,
                kegg.info = kegg.query,
                # due to the reason of database did not
                # have this kegg compound its standard spectrum, so that
                # align using identify metabolite its spectrum matrix
                align     = result,
                name      = feature$ID
            );
        } else {
            NULL;
        }
    });

    # get all non-null result
    query.result <- query.result[!sapply(query.result, is.null)];
    # and then get all names of the non-null result
    rows <- sapply(query.result, function(q) q$name) %=>% as.character;
    names(query.result) <- rows;

    query.result;
}

#' Pick the best alignment result
#'
#' @description Pick the best alignment result for unknown metabolite.
#'
#' @param ref The identify metabolite ms2 matrix
#' @param peak The unknown metabolite ms2 matrix set
#' @param ms2.align Method for alignment of the ms2 matrix
#'
#' @return Returns the alignment result, which is a R list object with members:
#'
#'     \code{list((string)[unknown feature]ms2.name, (ms2.matrix)[unknown feature]candidate, score = [forward, reverse])}
#'
#'     If the forward and reverse score cutoff less than score.cutoff, then this
#'     function will returns nothing.
#'
align_best.internal <- function(ref, peak, ms2.align, score.cutoff = 0.8, parent = NA) {

    # Error in `colnames<-`(`*tmp*`, value = c("ProductMz", "LibraryIntensity")) :
    #  attempt to set 'colnames' on an object with less than two dimensions
    if (is.null(nrow(ref))) {
        # 2019-2-26
        #
        # R language have a bug about matrix subset: matrix subset with only one row
        # will transform as vector automatic.
        # This will cause all of the matrix operation failure.
        #
        # using rbind to fix this bug.
        ref <- rbind(ref);
    }

    colnames(ref) <- c("ProductMz", "LibraryIntensity");

    # loop each unknown ms2 for alignment best result
    align <- lapply(names(peak), function(fileName) {
        file <- peak[[fileName]];

        # one raw file contains multiple ms2 scans data
        # alignment of the seeds spectra data
        # with each scan in current raw file.
        lapply(names(file), function(scan) {
            if (parent == sprintf("%s#%s", fileName, scan)) {
                # This is a self alignment...
                NULL;
            } else {
                unknown      <- file[[scan]];
                align.scores <- ms2.align(unknown, ref);
                ms2.name     <- list(
                    file = fileName,
                    scan = scan
                );

                if (all(align.scores >= score.cutoff)) {
                    # [ms2matrix<unknown feature>, reference<unknown>, score]
                    list(
                        score     = align.scores,
                        ms2.name  = ms2.name,
                        candidate = unknown
                    );
                } else {
                    NULL;
                }
            }
        });
    });

    align %=>% pickbest.internal;
}

#' Pick best alignment result for single ms1 feature
#'
#' @description due to the reason of all of the ms2 spectra in current one peak pack
#' is corresponding to one ms1 feature, so that the precursor m/z and rt
#' all the same. so no needs for rt.adjust score at here
#'
#' we just required compare the best dot product score at here
#'
#' @param align The alignment result of the ms2 spectra that align with reference spectra
#'
pickbest.internal <- function(align) {
    best.score <- -10000
    score      <- c();
    candidate  <- NULL;
    ms2.name   <- list();

    for(file in align) {
        for(scan in file) {
            if (!is.null(scan)) {

                x    <- scan$score;
                test <- min(x);

                if (test > best.score) {
                    score      <- x;
                    best.score <- test;
                    candidate  <- scan$candidate;
                    ms2.name   <- scan$ms2.name;
                }
            }
        }
    }

    if (!IsNothing(score)) {
        list(candidate = candidate,
             score     = score,
             ms2.name  = ms2.name
        );
    } else {
        NULL;
    }
}
