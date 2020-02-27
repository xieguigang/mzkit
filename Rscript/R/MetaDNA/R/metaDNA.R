#Region "Microsoft.ROpen::1053e42e75f660b3721cd9bff12cfbdd, metaDNA.R"

    # Summaries:

    # metaDNA <- function(identify, unknown, do.align,precursor_type = c("[M+H]+", "[M]+"),tolerance = assert.deltaMass(0.3),rt.adjust = function(rt, KEGG_id) 1,score.cutoff = 0.8,kegg_id.skips = NULL,seeds.all = TRUE,seeds.topn = 5,iterations = 20) {...
    # metaDNA.iteration <- function(identify, filter.skips,unknown, do.align,match.kegg,score.cutoff) {do.Predicts <- function(KEGG_cpd, identified, KEGG.partners, unknown.query) {do.infer <- function(seed) {...
    # create_filter.skips <- function(kegg_id.skips, debug.echo = TRUE) {if (kegg_id.skips %=>% IsNothing) {...
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
#' @param identify The metaDNA seeds data input, A \code{list} object with two members
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
#'      Where \code{q} and \code{s} is a ms2 spectra matrix. Due to the reason of the \code{q}
#'      and \code{s} spectra data is comes from the same sample and same instrument, so that
#'      using the \code{SSM} spectra alignment algorithm for this parameter is recommended.
#'
#' @param kegg_id.skips You can put the kegg compound ids in this character vector
#'       If you don't want some specific metabolite was indeified from this
#'       metaDNA algorithm function.
#' @param rt.adjust The rt adjust lambda function, it takes two parameter:
#'    \enumerate{
#'        \item \code{rt}, the rt of unidentified sample ms1 feature,
#'        \item \code{KEGG_id} the KEGG_cpd that assigned to current unidentified sample ms1 feature.
#'    }
#'    and then returns a rt alignment score. If this parameter is ignored,
#'    then will use score 1 as default means no rt adjustment score.
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
					rt.adjust = function(rt, KEGG_id) 1,
                    score.cutoff = 0.8,
                    kegg_id.skips = NULL,
					seeds.all = TRUE,
					seeds.topn = 5,
                    iterations = 20, 
					network.class_links = NULL) {

	require(foreach);
	require(doParallel);

    cat("\n");

	if (IsNothing(network.class_links)) {
		# use default kegg reaction class network data
		# data/metaDNA_kegg.rda
		xLoad("metaDNA_kegg.rda");

		network.class_links <- network;
	}

	network <- network.class_links;

    print(sprintf(" RUN [%s] pipline....", network$name));
    cat("\n");
	
	print(network$description);
	print(network$built);
	
	network.class_links <- network$network;

    # 1. Find all of the related KEGG compound by KEGG reaction link for
    #    identify metabolites
    # 2. Search for unknown by using ms1 precursor_m/z compare with the
    #    KEGG compound molecule weight in a given precursor_type mode.
    print("KEGG compound match with tolerance:");
    print(tolerance);

	timer        <- benchmark();
	unknown.mz   <- sapply(unknown, function(x) x$mz) %=>% as.numeric;
	filter.skips <- kegg_id.skips %=>% create_filter.skips;

	# The match.kegg lambda function implements:
	#
	# Query the unknown ms feature by a given KEGG partner id list
	#
    match.kegg <- kegg.match.handler(
      unknown.mz     = unknown.mz,
      precursor_type = precursor_type,
      tolerance      = tolerance
    );

	if (seeds.all && seeds.topn > 0) {
		print(sprintf("metaDNA only used top %s best hit as seeds...", seeds.topn));
	} else if (seeds.all) {
		print("metaDNA will used all of the hits as seeds...");
	} else {
		print("metaDNA will used the top best hit as seeds...");
	}

	print("do First iteration....");

    output <- metaDNA.iteration(
        identify, filter.skips,
        unknown, do.align,
        match.kegg,
        score.cutoff,
		network = network.class_links
    );

	memory.sample("[metaDNA]    do First iteration...");

	seeds         <- extends.seeds(output, rt.adjust, seeds.all, seeds.topn = seeds.topn);
	metaDNA.out   <- output;
	stats         <- NULL;
	totals        <- 0;
	kegg_id.skips <- append(kegg_id.skips, names(seeds));
	filter.skips  <- kegg_id.skips %=>% create_filter.skips;

	n      <- length(seeds);
	totals <- totals + n;
	stats  <- rbind(stats, c(0, n, totals, timer()$since_last));

	if (iterations > 1) {
	    for(i in 1:iterations) {
			print(sprintf("   do metaDNA Iteration %s ...", i));

			output <- metaDNA.iteration(
				identify     = seeds,
				filter.skips = filter.skips,
				unknown      = unknown,
				do.align     = do.align,
				match.kegg   = match.kegg,
				score.cutoff = score.cutoff,
				network      = network.class_links
			);
			metaDNA.out <- append(metaDNA.out, output);

			# using identify output as seeds for next iteration
			seeds <- extends.seeds(output, rt.adjust, seeds.all, seeds.topn = seeds.topn);
			n <- length(seeds);

			if (n == 0) {
				if (debug.echo) {
					print("No more metabolite can be predicted, exit iterations...");
				}

				break;
			} else {
				print(sprintf("  Found %s kegg compound:", n));
				# print(names(seeds));

				kegg_id.skips <- append(kegg_id.skips, names(seeds));
				filter.skips  <- create_filter.skips(kegg_id.skips, FALSE);
				totals        <- totals + n;
				stats         <- rbind(stats, c(i, n, totals, timer()$since_last));
			}

			memory.sample(sprintf("[metaDNA]    do metaDNA Iteration %s ...", i));
		}

		if (!is.null(stats)) {
			colnames(stats) <- c("Iteration", "Predicts", "Total", "Elapsed(ms)");
			rownames(stats) <- stats[, "Iteration"];

			print(stats);
		}
	}

    # at last returns the prediction result
    metaDNA.out;
}

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

	do.Predicts <- function(KEGG_cpd, identified, KEGG.partners, unknown.query) {
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
		infer <- if (length(identified) > (2 * MetaDNA::cluster.cores())) {
			# parallel
			envir.exports <- c("unknown.query", "unknown", "do.align", "score.cutoff", "do.infer");
			cl <- makeCluster(MetaDNA::cluster.cores());
			registerDoParallel(cl);

			output <- foreach(seed = identified, .export = envir.exports) %dopar% {
				do.infer(seed);
			}

			stopCluster(cl);
			output;
		} else {
			lapply(identified, do.infer);
		}

		# returns the metaDNA network infer result
		# of current iteration.
		infer;
	}

    # tick.each
    # lapply
	seeds.KEGG_id <- names(identify);
    seeds <- tick.each(seeds.KEGG_id, function(KEGG_cpd) {

        # Get all of the kegg reaction partner metabolite id
        # for current identified kegg metabolite id
		#
		# this seeds data contains multiple hits
		if (KEGG_cpd %=>% IsNothing) {
			NULL;
		} else {

		    identified <- identify[[KEGG_cpd]];
			# find KEGG reaction partner for current identify KEGG compound
			KEGG.partners <- kegg.partners(KEGG_cpd, network) %=>% filter.skips %=>% unique;

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
					do.Predicts(KEGG_cpd, identified, KEGG.partners, unknown.query);
				}
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
create_filter.skips <- function(kegg_id.skips, debug.echo = TRUE) {

    if (kegg_id.skips %=>% IsNothing) {
        kegg_id.skips = "NA";
    } else if (debug.echo) {
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
