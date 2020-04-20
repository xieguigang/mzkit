#Region "Microsoft.ROpen::2ec05047309feb9c13ef66c73cbeb3d5, metaDNA_iteration.R"

    # Summaries:

    # metaDNA.iteration <- function(identify, filter.skips,unknown, do.align,match.kegg,score.cutoff,network) {...
    # do.Predicts <- function(KEGG_cpd, identified, KEGG.partners, unknown.query, unknown, do.align, score.cutoff) {do.infer <- function(seed) {...

#End Region

#' Run a metaDNA prediction iteration
#'
#' @param identify The seeds data for the metaDNA algorithm.
#' @param kegg.partners A lambda function for find reaction partners for a given list of
#'        KEGG compound id.
#' @param unknown The user sample data
#' @param do.align A lambda function that provides spectra alignment
#' @param unknow.matches function evaluate result of \code{\link{kegg.match.handler}},
#'     this function descript that how to find out the unknown metabolite from a given
#'     set of identify related kegg partners compound id set.
#'
#' @details The \code{do.align} function should take two parameter:
#'     The spectra matrix of query and reference and retuns a score vector
#'     which produced by forward and reverse spectra alignment.
#'
metaDNA.iteration <- function(identify, filter.skips,
                              unknown, do.align,
                              match.kegg,
                              score.cutoff,
                              network) {
    # tick.each
    # lapply
    seeds.KEGG_id <- names(identify);
    seeds <- tick.each(seeds.KEGG_id, function(KEGG_cpd) {

        # Get all of the kegg reaction partner metabolite id
        # for current identified kegg metabolite id
        #
        # this seeds data contains multiple hits
        if (KEGG_cpd %=>% IsNothing) {
            return(NULL);
        }

        identified <- identify[[KEGG_cpd]];
        # find KEGG reaction partner for current identify KEGG compound
        KEGG.partners <- kegg.partners(KEGG_cpd, network) %=>% filter.skips %=>% unique;

        # current identify metabolite KEGG id didnt found any
        # reaction related partner compounds
        # Skip current identify metabolite.
        if (KEGG.partners %=>% IsNothing) {
            return(NULL);
        } else if (length(identified) == 0) {
            return(NULL);
        }

        # identify contains single result
        # Each metaDNA.impl result is a list that identify of
        # unknowns

        # Current set of KEGG.partners which comes from the identify KEGG metabolite
        # can have multiple unknown metabolite match result
        #
        # precursor_type list();
        unknown.query <- KEGG.partners %=>% match.kegg;

        if (IsNothing(unknown.query)) {
            NULL;
        } else {
            do.Predicts(KEGG_cpd, identified, KEGG.partners, unknown.query, unknown, do.align, score.cutoff);
        }
    });

    rm(identify);
    rm(unknown);

    seeds;
}

#' do metaDNA prediction for given KEGG compounds
#'
#' @param KEGG_cpd the kegg compound id
#' @param do.align a lambda function for evaluate align score of two spectra matrix.
#'
do.Predicts <- function(KEGG_cpd, identified, KEGG.partners, unknown.query, unknown, do.align, score.cutoff) {
    do.infer <- function(seed) {
        # get trace information of current seed:
        #
        # The seed$feature is the ms1 feature id of the identified seed
        # The seed$ref is the ms2 index of the identified seed, used for retrive
        # the ms2 spectrum mnatrix data
        trace <- seed$ref;
        trace <- list(
            # The unknown infer elongation path
            path   = seed$trace %||% (seed %=>% trace.node),
            KEGG   = seed$KEGG,
            # ms feature of current seed
            parent = sprintf("%s#%s", trace$file, trace$scan),
            ref    = seed$feature
        );

        # do iteration
        metaDNA.impl(
            unknown.query = unknown.query,
            identify.ms2  = seed$spectra,
            # trace path is debug used only
            # to visualize how the seeds extends to
            # other metabolite in KEGG reaction
            # network
            trace         = trace,
            unknown       = unknown,
            ms2.align     = do.align,
            score.cutoff  = score.cutoff
        );
    }

    # element structure in unknown.query:
    #
    # [1] "unknown.index"  "unknown.mz"     "precursor_type" "kegg"
    # [5] "ppm"
    #
    # unknown.index is the index of the unknown metabolite in input sequence
    # unknown.mz is the corresponding m/z
    # ppm is the ppm value for unknown mz match with the KEGG compound m/z
    # KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches

    if (length(identified) > 1) {

        # parallel
        envir.exports <- c("unknown.query", "unknown", "do.align", "score.cutoff", "do.infer");
        cl <- makeCluster(min(MetaDNA::cluster.cores(), length(identified)));
        registerDoParallel(cl);

        infer <- foreach(seed = identified, .export = envir.exports) %dopar% {
            do.infer(seed);
        }

        rm(identified);
        rm(unknown);
        rm(unknown.query);

        stopCluster(cl);

    } else {
        infer <- lapply(identified, do.infer);
    }

    # returns the metaDNA network infer result
    # of current iteration.
    infer;
}
