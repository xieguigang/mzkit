#Region "Microsoft.ROpen::b2ba9de6f2eb9ee6895ae9b055bc9f5e, metaDNA.R"

    # Summaries:

    # metaDNA <- function(identify, unknown, do.align,precursor_type = c("[M+H]+", "[M]+"),tolerance = assert.deltaMass(0.3),score.cutoff = 0.8,kegg_id.skips = NULL,iterations = 20) {...
    # metaDNA.iteration <- function(identify, kegg.partners, filter.skips,unknown, do.align,match.kegg,score.cutoff) {# tick.each# lapplyseeds <- tick.each(names(identify), function(KEGG_cpd) {...
    # create_filter.skips <- function(kegg_id.skips) {if (kegg_id.skips %=>% IsNothing) {...
    # filter.skips <- function(partners) {if (partners %=>% IsNothing) {...

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
	
	print("do First iteration....");

    output <- metaDNA.iteration(
        identify, filter.skips,
        unknown, do.align,
        match.kegg,
        score.cutoff
    );
	seeds <- extends.seeds(output);
	metaDNA.out <- output;
	stats <- NULL;
	totals <- 0;
	kegg_id.skips <- append(kegg_id.skips, names(seeds));
	filter.skips <- kegg_id.skips %=>% create_filter.skips;
	
    for(i in 1:iterations) {		
		print(sprintf("   do metaDNA Iteration %s ...", i));
	
        output <- metaDNA.iteration(
            identify = seeds,            
            filter.skips = filter.skips,
            unknown = unknown,
            do.align = do.align,
            match.kegg = match.kegg,
            score.cutoff = score.cutoff
        );
		metaDNA.out <- append(metaDNA.out, output);				
		
		# using identify output as seeds for next iteration
		seeds <- extends.seeds(output);
		n <- length(seeds);
		
		if (n == 0) {
			if (debug.echo) {
				print("No more metabolite can be predicted, exit iterations...");
			}
		
			break;
		} else {
			print(sprintf("  Found %s kegg compound:", n));
			print(names(seeds));
			
			kegg_id.skips <- append(kegg_id.skips, names(seeds));
			filter.skips <- kegg_id.skips %=>% create_filter.skips;			
			totals <- totals + n;
			stats <- rbind(stats, c(i, n, totals));
		}
    }

	colnames(stats) <- c("Iteration", "Predicts", "Total");
	rownames(stats) <- stats[, "Iteration"];
	
	print(stats);
	
    # at last returns the prediction result
    metaDNA.out;
}

#' Convert output as metaDNA seeds
#'
#' @description The identify list only provides ms2 spectra matrix and KEGG id.
#'   KEGG id is the names of the identify list.
#'   So, the identify list object its structure looks like: 
#' 
#'   \code{
#'      identify \{
#'         KEGG_id1 => list(spectra),
#'         KEGG_id2 => list(spectra),
#'         ...
#'      \}
#'   }
#'
extends.seeds <- function(output) {
	# one kegg id only have one best spectra
	seeds <- list();
	
	for(block in output) {
		if (block %=>% IsNothing) {
			next;
		}
	
		for(feature in block) {
			KEGG <- feature$kegg.info$kegg$ID;
			best <- seeds[[KEGG]];
			hit <- list(
				spectra = feature$align$candidate,
				score = feature$align$score
			);
			
			if (best %=>% IsNothing) {
				# current feature alignment is the best
				seeds[[KEGG]] <- hit;
			} else {
				if (min(hit$score) > min(best$score)) {
					# current feature alignment is the best alignment
					seeds[[KEGG]] <- hit;
				} else {
					# no changes
				}
			}
		}
	}
	
	seeds;
}

#' Run a metaDNA prediction iteration
#'
#' @param identify The seeds data for the metaDNA algorithm.
#' @param kegg.partners A lambda function for find reaction partners for a given list of KEGG compound id.
#' @param unknown The user sample data
#' @param do.align A lambda function that provides spectra alignment
#' @param unknow.matches function evaluate result of \code{\link{kegg.match.handler}}, this function
#'     descript that how to find out the unknown metabolite from a given set of identify related kegg
#'     partners compound id set.
#'
#' @details The \code{do.align} function should take two parameter:
#'     The spectra matrix of query and reference and retuns a score vector
#'     which produced by forward and reverse spectra alignment.
#'
metaDNA.iteration <- function(identify, filter.skips,
                              unknown, do.align,
                              match.kegg,
                              score.cutoff) {
    # tick.each
    # lapply
    seeds <- tick.each(names(identify), function(KEGG_cpd) {
        # Get all of the kegg reaction partner metabolite id
        # for current identified kegg metabolite id
        identified <- identify[[KEGG_cpd]];
		# find KEGG reaction partner for current identify KEGG compound
        KEGG.partners <- KEGG_cpd %=>% kegg.partners %=>% filter.skips %=>% unique;

        # current identify metabolite KEGG id didnt found any
        # reaction related partner compounds
        # Skip current identify metabolite.
        if (KEGG.partners %=>% IsNothing) {
            NULL;
        } else {

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
                # element structure in unknown.query:
                #
                # [1] "unknown.index"  "unknown.mz"     "precursor_type" "kegg"
                # [5] "ppm"
                #
                # unknown.index is the index of the unknown metabolite in input sequence
                # unknown.mz is the corresponding m/z
                # ppm is the ppm value for unknown mz match with the KEGG compound m/z
                # KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches
                metaDNA.impl(
                    unknown.query = unknown.query,
                    identify.ms2 = identified$spectra,
                    unknown = unknown,
                    ms2.align = do.align,
                    score.cutoff = score.cutoff
                );
            }
        }
    });

    seeds;
}

#' Create skips handler for KEGG id
#'
#' @param kegg_id.skips A character vector that contains KEGG id
#'   want to ignores in the metaDNA prediction process.
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
