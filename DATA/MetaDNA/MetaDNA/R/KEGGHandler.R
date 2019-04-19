#Region "Microsoft.ROpen::6e9fdd583cfe79dc22821fd5f357d75f, KEGGHandler.R"

    # Summaries:

    # kegg.match.handler <- function(unknown.mz, precursor_type = c("[M+H]+", "[M]+"), tolerance = assert.deltaMass(0.3)) {...
    # kegg.match <- function(kegg_id, kegg.mass, kegg.ids, kegg.mz, kegg.list,precursor_type,unknown.mz,tolerance) {...
    # unknown.query_impl <- function(ms1, j) {query <- lapply(mz.index, function(i) {	# unknown metabolite ms1 m/z match	# kegg mz with a given tolerance	if ((!IsNothing(mz[i])) && tolerance(ms1, mz[i])) {...
    # kegg.partners <- function(kegg_id) {...

#End Region

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
			if ((!IsNothing(mz[i])) && tolerance(ms1, mz[i])) {
				# If these two m/z value meet the tolerance condition
				# then we match a possible KEGG annotation data.
				# also returns with ppm value
				list(kegg = kegg[[i]], ppm = PPM(ms1, mz[i]), libmz = mz[i]);
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
					ppm = hit$ppm,
					libmz = hit$libmz
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
