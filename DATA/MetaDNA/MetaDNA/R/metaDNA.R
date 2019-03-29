#Region "Microsoft.ROpen::7884176049d5543d31c91cff233bbb3c, metaDNA.R"

    # Summaries:

    # metaDNA <- function(identify, unknown, meta.KEGG, ms2.align,precursor_type = c("[M+H]+", "[M]+"),tolerance = assert.deltaMass(0.3),score.cutoff = 0.8,kegg_id.skips = NULL) {...
    # filter.skips <- function(partners) {if (partners %=>% IsNothing) {...
    # kegg.match.handler <- function(meta.KEGG, unknown.mz,precursor_type = c("[M+H]+", "[M]+"),kegg_id = "KEGG",tolerance = assert.deltaMass(0.3)) {...
    # kegg.match <- function(kegg_id, kegg.mass, kegg.ids, kegg.mz, kegg.list, precursor_type, tolerance) {...
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
                    kegg_id.skips = NULL) {

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
    match.kegg <- kegg.match.handler(
      unknown.mz = unknown.mz,
      precursor_type = precursor_type,      
      tolerance = tolerance
    );
    
    if (kegg_id.skips %=>% IsNothing) {
      kegg_id.skips = "NA";
    } else {
      cat("\nThese KEGG compound will not be identified from metaDNA\n\n");
      print(kegg_id.skips);
      cat("\n");
    }

	kegg_id.skips <- as.index(kegg_id.skips);

    # return the kegg id vector which is not in
    # kegg_id.skips
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

    # tick.each
    # lapply
    tick.each(names(identify), function(KEGG_cpd) {
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
}

#' Match unknown by mass
#'
#' @param unknown.mz This mz parameter is a \code{m/z} vector from the
#'         \code{unknown$peaktable}
#' @param precursor_type precursor type that using for calculate
#'         \code{m/z} from KEGG metabolite mass value.
#'
#' @return Returns the index vector in \code{unknown.mz} vector.
#' @details \code{mass - mz - kegg_id}
kegg.match.handler <- function(unknown.mz, precursor_type = c("[M+H]+", "[M]+"), tolerance = assert.deltaMass(0.3)) {
    # load kegg reaction database and kegg meta information
    # data/metaDNA_kegg.rda
    xLoad("metaDNA_kegg.rda");
	xLoad("KEGG_meta.rda");
	
	# 2019-03-29 some metabolite in KEGG database is a generic compound
	# which means it have no mass and formula
	# removes all of these generic compound.
	non.generic <- sapply(KEGG_meta, function(c) c$exact_mass > 0) %=>% as.logical;
	KEGG_meta <- KEGG_meta[non.generic];
	
	mode <- getPolarity(precursor_type);

	print("m/z will be calculate from these precursor types:");
	print(cbind(precursor_type, mode));

    kegg.mass <- sapply(KEGG_meta, function(c) c$exact_mass) %=>% as.numeric;
    kegg.ids <- sapply(KEGG_meta, function(c) c$ID) %=>% as.character;
    kegg.mz <- get.PrecursorMZ(kegg.mass, precursor_type, mode);
    kegg.list <- KEGG_meta;

    function(kegg_id) {
		out <- list();
		i <- 1;
		list <- lapply(precursor_type, function(type) {
			# mz <- kegg.mz[[type]];
			kegg.match(
				kegg_id, kegg.mass, kegg.ids, kegg.mz, kegg.list, 
				precursor_type = type, 
				unknown.mz     = unknown.mz, 
				tolerance      = tolerance
			);
		});

		for(hits in list) {
			if (!is.null(hits)) {
				for(kegg in hits) {
					out[[i]] <- kegg;
					i = i + 1;
				}
			}
		}

		if (length(out) == 0) {
			NULL;
		} else {
			names(out) <- as.character(1:length(out));
			out;
		}
    }
}

# identify kegg partners
#   => kegg m/z
#   => unknown mz with tolerance
#   => unknown index
#   => unknown peak and ms2 for align
kegg.match <- function(kegg_id, kegg.mass, kegg.ids, kegg.mz, kegg.list, 
	precursor_type, 
	unknown.mz, 
	tolerance) {

	# Get kegg m/z for a given kegg_id set
	kegg_id <- as.index(kegg_id);
	mzi <- sapply(kegg.ids, kegg_id) %=>% as.logical;
	# Get corresponding kegg mz and annotation meta data
	mz <- kegg.mz[[precursor_type]][mzi];
	kegg <- kegg.list[mzi];
	mz.index <- 1:length(mz);

	# the ms1 parameter is a m/z number of a unknown metabolite
	# j is the index of the unknown m/z in the vector 
	unknown.query_impl <- function(ms1, j) {
		query <- lapply(mz.index, function(i) {
			# unknown metabolite ms1 m/z match
			# kegg mz with a given tolerance
			if (tolerance(ms1, mz[i])) {
				# If these two m/z value meet the tolerance condition
				# then we match a possible KEGG annotation data.
				# also returns with ppm value
				list(kegg = kegg[[i]], ppm = PPM(ms1, mz[i]));
			} else {
				NULL;
			}
		});

		# removes all null result
		nulls <- sapply(query, is.null) %=>% unlist;
		query <- query[!nulls];

		if (length(query) == 0) {
			NULL;
		} else {
			lapply(query, function(hit) {
				list(
					unknown.index = j,
					unknown.mz = ms1,
					precursor_type = precursor_type,

					# current unknown metabolite could have
					# multiple kegg annotation result, based on the ms1
					# tolerance.
					kegg = hit$kegg,
					ppm = hit$ppm
				);
			});
		}
	}

	# Loop on each unknown metabolite ms1 m/z
	# And using this m/z for match kegg m/z to
	# get possible kegg meta annotation data.
	unknown.query <- sapply(1:length(unknown.mz), function(j) {
		ms1 <- unknown.mz[j];

		# 2018-7-8 why NA value happened???
		if (is.na(ms1)) {
			NULL;
		} else {
			unknown.query_impl(ms1, j);
		}
	});

	# removes null result
	# Get null index and then removes null subset
	nulls <- sapply(unknown.query, is.null) %=>% unlist;
	unknown.query <- unknown.query[!nulls];

	if (length(unknown.query) == 0) {
		NULL;
	} else {
		out <- list();
		i <- 1;

		for(query in unknown.query) {
			for(hit in query) {
				out[[i]] <- hit;
				i = i + 1;
			}
		}

		names(out) <- as.character(1:length(out));
		out;
	}
}

#' Find kegg reaction partner
#'
#' @description Find KEGG reaction partner compound based on the kegg
#'     reaction definition.
#'
#' @param kegg_id The kegg compound id of the identified compound.
#'
#' @return A kegg id vector which is related to the given \code{kegg_id}.
#'
kegg.partners <- function(kegg_id) {
	# kegg_id list could be empty???
	is_null <- IsNothing(kegg_id);

    sapply(network, function(reaction) {
		# 2019-03-01
		# fix for Error in if (kegg_id %in% reaction$reactants) { :
		if (is_null) {
			NULL;
		} else if (kegg_id %in% reaction$reactants) {
            reaction$products;
        } else if (kegg_id %in% reaction$products) {
            reaction$reactants;
        } else {
            NULL;
        }

    }) %=>% unlist %=>% as.character;
}

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
#' @param unknow.matches function evaluate result of \code{\link{kegg.match.handler}}, this function
#'      descript that how to find out the unknown metabolite from a given set of identify related kegg
#'      partners compound id set.
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
metaDNA.impl <- function(KEGG.partners, identify.ms2,
                         unknown,
                         ms2.align,
                         unknow.matches,
                         score.cutoff = 0.8) {

    # Current set of KEGG.partners which comes from the identify KEGG metabolite
    # can have multiple unknown metabolite match result
	#
	# precursor_type list();
    unknown.query <- KEGG.partners %=>% unknow.matches;

    if (IsNothing(unknown.query)) {
        return(NULL);
    } else {
		# element structure in unknown.query:
		#
		# [1] "unknown.index"  "unknown.mz"     "precursor_type" "kegg"
		# [5] "ppm"
		#
		# unknown.index is the index of the unknown metabolite in input sequence
		# unknown.mz is the corresponding m/z
		# ppm is the ppm value for unknown mz match with the KEGG compound m/z
	}

    # unknown.i integer index of the peaktable
    unknown.i <- sapply(unknown.query, function(x) x$unknown.index) %=>% unlist %=>% as.numeric;
    # subset of the peaktable by using the unknown index value
	# the peaktable subset object contains ms1 feature and ms2 feature
    unknown.features <- unknown[unknown.i];
       
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
        peak <- feature$ms2;	
		result <- align_best.internal(
		  ref = identify.ms2,
		  peak = peak,
		  ms2.align = ms2.align,
		  score.cutoff = score.cutoff
		);

        if (!is.null(result)) {
            # name is the peaktable rownames
			feature$ms2 <- NULL;
			
            list(
              feature = feature,
              kegg.info = kegg.query,
			  # due to the reason of database did not 
			  # have this kegg compound its standard spectrum, so that 
			  # align using identify metabolite its spectrum matrix
			  align = result,
			  name = feature$ID
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
#'     \code{list((ms2.matrix)ref, (ms2.matrix)candidate, score = [forward, reverse])}
#'
#'     If the forward and reverse score cutoff less than score.cutoff, then this
#'     function will returns nothing.
#'
align_best.internal <- function(ref, peak, ms2.align, score.cutoff = 0.8) {

    best.score <- -10000
    score <- c();
    candidate <- NULL;
    ms2.name <- list();

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

    # loop each unknown for alignment best result
	align <- lapply(names(peak), function(fileName) {
		file <- peak[[fileName]];
		lapply(names(file), function(scan) {
			unknown <- file[[scan]];
			align.scores <- ms2.align(unknown, ref);
			ms2.name <- list(
				file = fileName,
				scan = scan
            );

			if (all(align.scores >= score.cutoff)) {
				list(score = align.scores, ms2.name = ms2.name, candidate = unknown);
			} else {
				NULL;
			}
		});
	});

	for(file in align) {
		for(scan in file) {
			if (!is.null(scan)) {
				x <- scan$score;
				test <- mean(x);

				if (test > best.score) {
					score <- x;
					best.score <- test;
					candidate <- scan$candidate;
					ms2.name <- scan$ms2.name;
				}
			}
		}
	}

    if (!IsNothing(score)) {
        list(ref = ref,
             candidate = candidate,
             score = score,
             ms2.name = ms2.name
        );
    } else {
        NULL;
    }
}
