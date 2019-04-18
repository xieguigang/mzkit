#Region "Microsoft.ROpen::f2279eaf7e80476c8957c76a49ea0926, metaDNA.R"

    # Summaries:

    # metaDNA <- function(identify, unknown, do.align,precursor_type = c("[M+H]+", "[M]+"),tolerance = assert.deltaMass(0.3),score.cutoff = 0.8,kegg_id.skips = NULL) {...
    # filter.skips <- function(partners) {if (partners %=>% IsNothing) {...
    # kegg.match.handler <- function(unknown.mz, precursor_type = c("[M+H]+", "[M]+"), tolerance = assert.deltaMass(0.3)) {...
    # kegg.match <- function(kegg_id, kegg.mass, kegg.ids, kegg.mz, kegg.list,precursor_type,unknown.mz,tolerance) {...
    # unknown.query_impl <- function(ms1, j) {query <- lapply(mz.index, function(i) {	# unknown metabolite ms1 m/z match	# kegg mz with a given tolerance	if (tolerance(ms1, mz[i])) {...
    # kegg.partners <- function(kegg_id) {...
    # metaDNA.impl <- function(KEGG.partners, identify.ms2,unknown,ms2.align,unknow.matches,score.cutoff = 0.8) {...
    # align_best.internal <- function(ref, peak, ms2.align, score.cutoff = 0.8) {...

#End Region

require(VisualBasic.R);

Imports(Microsoft.VisualBasic.Data);
Imports(Microsoft.VisualBasic.Data.Linq);
Imports(Microsoft.VisualBasic.Language);

#' Identify unknown metabolite by metaDNA algorithm
#'
#' @description How to: The basic idea of the \code{MetaDNA} algorightm is
#'      using the identified ms2 data to align the unknown ms2 data.
#'
#' @param identify A \code{list} object with two members
#'
#'      \code{identify = list((data.frame) meta.KEGG, (list) peak_ms2)}
#'
#'      where:
#'
#'         \code{meta.KEGG} is a data.frame object that should contains
#'               a KEGG id column, and a index column to read \code{peak_ms2} list.
#'
#'         \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'               least two column: \code{mz} and \code{into}
#'
#' @param unknown A \code{list} object with two members:
#'
#'      \code{unknown = list((data.frame) peaktable, (list) peak_ms2)}
#'
#'      where:
#'
#'      \code{peaktable} is a data.frame object that contains the MS1
#'      information: \code{mz} and \code{rt}, and a additional column index
#'      to read \code{peak_ms2} list.
#'
#'      \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'      least two column: \code{mz} and \code{into}
#'
#' @param precursor_type By default is positive mode with the \code{H+} adduct for
#'      search unknown metabolites.
#'
#' @param tolerance KEGG compound match with this tolerance, m/z equalient compares tolerance,
#'      by default is less than ppm 20.
#'
#' @param score.cutoff MS/MS similarity cutoff for identify ms2 alignment with unknown ms2
#'
#' @param do.align The MS/MS alignment method, which is in format like: \code{function(q, s)}
#'      Where \code{q} and \code{s} is a ms2 spectra matrix.
#'
#' @param kegg_id.skips You can put the kegg compound ids in this character vector
#'       If you don't want some specific metabolite was indeified from this
#'       metaDNA algorithm function.
#'
#' @return A \code{identify} parameter data structure like metabolite identify
#'      result for \code{unknown} parameter input data
#'
#' @details Algorithm implementation and details see: \code{\link{metaDNA.impl}}.
#'    The ms2 matrix should be in format like:
#'
#'     \code{
#'         mz into\cr
#'         xxx xxx\cr
#'         xxx xxx\cr
#'         xxx xxx\cr
#'     }
#'
metaDNA <- function(identify, unknown, do.align,
                    precursor_type = c("[M+H]+", "[M]+"),
                    tolerance = assert.deltaMass(0.3),
                    score.cutoff = 0.8,
                    kegg_id.skips = NULL,
                    iterations = 20) {

    cat("\n\n\n");

    # 1. Find all of the related KEGG compound by KEGG reaction link for
    #    identify metabolites
    # 2. Search for unknown by using ms1 precursor_m/z compare with the
    #    KEGG compound molecule weight in a given precursor_type mode.

    print(" [metaDNA] pipline....");
    cat("\n\n");
    print("KEGG compound match with tolerance:");
    print(tolerance);

	unknown.mz <- sapply(unknown, function(x) x$mz) %=>% as.numeric;
	filter.skips <- kegg_id.skips %=>% create_filter.skips;
    match.kegg <- kegg.match.handler(
      unknown.mz = unknown.mz,
      precursor_type = precursor_type,
      tolerance = tolerance
    );

    seeds <- metaDNA.iteration(identify, kegg.partners, filter.skips, unknown, do.align, match.kegg, score.cutoff);

    for(i in 1:iterations) {
        seeds <- metaDNA.iteration(
            identify = seeds,
            kegg.partners = kegg.partners,
            filter.skips = filter.skips,
            unknown = unknown,
            do.align = do.align,
            match.kegg = match.kegg,
            score.cutoff = score.cutoff
        );
    }

    # at last returns the prediction result
    seeds;
}

metaDNA.iteration <- function(identify, kegg.partners, filter.skips, unknown, do.align, match.kegg, score.cutoff) {
    # tick.each
    # lapply
    seeds <- tick.each(names(identify), function(KEGG_cpd) {
        # Get all of the kegg reaction partner metabolite id
        # for current identified kegg metabolite id
        identified <- identify[[KEGG_cpd]];
        partners <- KEGG_cpd %=>% kegg.partners %=>% filter.skips %=>% unique;

        # current identify metabolite KEGG id didnt found any
        # reaction related partner compounds
        # Skip current identify metabolite.
        if (partners %=>% IsNothing) {
            NULL;
        } else {

            # identify contains single result
            # Each metaDNA.impl result is a list that identify of
            # unknowns

            # KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches
            metaDNA.impl(
                KEGG.partners = partners,
                identify.ms2 = identified$spectra,
                unknown = unknown,
                ms2.align = do.align,
                unknow.matches = match.kegg,
                score.cutoff = score.cutoff
            );
        }
    });

    seeds;
}

#' Create skips handler for KEGG id
#'
#' @param kegg_id.skips A character vector that contains KEGG id
#' want to ignores in the metaDNA prediction process.
#'
#' @return This function returns a lambda function that can determine the
#'   given kegg id vector which is not in the input \code{kegg_id.skips}.
#'
create_filter.skips <- function(kegg_id.skips) {

    if (kegg_id.skips %=>% IsNothing) {
        kegg_id.skips = "NA";
    } else {
        cat("\nThese KEGG compound will not be identified from metaDNA\n\n");
        print(kegg_id.skips);
        cat("\n");
    }

    kegg_id.skips <- as.index(kegg_id.skips);
    filter.skips <- function(partners) {
        if (partners %=>% IsNothing) {
            NULL;
        } else {

            # if the partner id is in the skip list
            # then it will be replaced as string "NA"
            partners <- sapply(partners, function(id) {
                if (kegg_id.skips(id)) {
                    "NA";
                } else {
                    id;
                }
            }) %=>% as.character;

            # Removes those NA string in the partner id list
            partners[partners != "NA"];
        }
    }

    filter.skips;
}
