#Region "Microsoft.ROpen::8588f59e2553ca2dcad4a36eb473a078, KEGG_handler.R"

    # Summaries:

    # Delete.EmptyKEGG <- function(dataframe, col.name = "KEGG") {...
    # kegg.match.handler <- function(unknown.mz,precursor_type = c("[M+H]+", "[M]+"),tolerance = assert.deltaMass(0.3),libtype   = 1) {...
    # get.kegg_precursorMZ <- function(KEGG_meta, precursor_type, mode) {getMz <- function(type) {    sapply(KEGG_meta, function(cpd) {   if (mode == 1) {...
    # kegg.match <- function(partners_id, kegg.mass, kegg.ids, kegg.mz, kegg.list,precursor_type,unknown.mz,tolerance) {...
    # unknown.query_impl <- function(ms1, j) {# mz.index is 1 -> lengthOf(kegg_matched mz)# the ppm value between unknown ms1 m/z and kegg m/z# was calculated in the query lapply loopquery <- lapply(mz.index, function(i) {    # unknown metabolite ms1 m/z match    # kegg mz with a given tolerance    if ((!IsNothing(mz[i])) && tolerance(ms1, mz[i])) {...
    # kegg.partners <- function(kegg_id, network) {...

#End Region

#' Removes empty \code{KEGG} row
#'
#' @description Removes all of the rows in a given \code{dataframe}
#'      which its \code{KEGG} column value is string empty.
#'
#' @param d A \code{dataframe} object which contains the metabolite
#'      annotation result data. This dataframe must contains a column
#'      which is named \code{KEGG}.
#'
#' @return A subset of the input \code{dataframe} with all KEGG column
#'     value non-empty.
Delete.EmptyKEGG <- function(dataframe, col.name = "KEGG") {
    KEGG <- dataframe[, col.name] %=>% as.vector;
    test <- !Strings.Empty(KEGG, TRUE);

    dataframe[test, ];
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
#'
kegg.match.handler <- function(
    unknown.mz,
    precursor_type = c("[M+H]+", "[M]+"),
    tolerance      = assert.deltaMass(0.3),
    libtype        = 1) {

    # load kegg reaction database and kegg meta information
    xLoad("KEGG_meta.rda");

    # 2019-03-29 some metabolite in KEGG database is a generic compound
    # which means it have no mass and formula
    # removes all of these generic compound.
    non.generic <- sapply(KEGG_meta, function(c) c$exact_mass > 0) %=>% as.logical;
    KEGG_meta   <- KEGG_meta[non.generic];

    # debug used
    mode <- getPolarity(precursor_type);

    print("m/z will be calculate from these precursor types:");
    print(cbind(cbind(precursor_type, mode), libtype));

    # kegg.mass and kegg.ids have the same element length and
    # keeps the same element orders
    kegg.mass <- sapply(KEGG_meta, function(c) c$exact_mass) %=>% as.numeric;
    kegg.ids  <- sapply(KEGG_meta, function(c) c$ID) %=>% as.character;
    kegg.mz   <- get.kegg_precursorMZ(KEGG_meta, precursor_type, mode = libtype);
    kegg.list <- KEGG_meta;

    function(kegg_id) {
        out  <- list();
        i    <- 1;
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

#' Get precursor m/z data list
#'
#' @param KEGG_meta The kegg meta dataset that loaded from the metaDNA package
#' @param precursor_type A character vector data for get m/z from the precursor data matrix
#' @param mode libtype value, value could be 1 or -1
#'
get.kegg_precursorMZ <- function(KEGG_meta, precursor_type, mode) {
    getMz <- function(type) {
        sapply(KEGG_meta, function(cpd) {
            if (mode == 1) {
                matrix <- cpd$positive;
            } else {
                matrix <- cpd$negative;
            }

            x <- matrix[type, "mz"];

            if (is.na(x)) {
                0;
            } else {
                x;
            }
        });
    };
    mz <- lapply(precursor_type, function(type) {
        as.vector(as.numeric(getMz(type)));
    });
    names(mz) <- precursor_type;

    mz;
}

# identify kegg partners
#   => kegg m/z
#   => unknown mz with tolerance
#   => unknown index
#   => unknown peak and ms2 for align

#' Match unknown feature from given KEGG partners
#'
#' @param partners_id The kegg partner compound id list, which is query
#'     from the identifed KEGG seed by the reaction network definition.
#'
kegg.match <- function(partners_id, kegg.mass, kegg.ids, kegg.mz, kegg.list,
                       precursor_type,
                       unknown.mz,
                       tolerance) {

    # Get index of kegg m/z for a given kegg_id set
    mzi      <- (kegg.ids %in% partners_id);
    # Get corresponding kegg mz and annotation meta data
    # in given precursor_type kegg m/z calculation data
    mz       <- kegg.mz[[precursor_type]][mzi];
    # Get corresponding kegg meta info by a given matched index list
    kegg     <- kegg.list[mzi];
    mz.index <- 1:length(mz);

    # the ms1 parameter is a m/z number of a unknown metabolite
    # j is the index of the unknown m/z in the vector
    unknown.query_impl <- function(ms1, j) {
        # mz.index is 1 -> lengthOf(kegg_matched mz)
        # the ppm value between unknown ms1 m/z and kegg m/z
        # was calculated in the query lapply loop
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
                    unknown.index  = j,
                    unknown.mz     = ms1,
                    precursor_type = precursor_type,

                    # current unknown metabolite could have
                    # multiple kegg annotation result, based on the ms1
                    # tolerance.
                    kegg  = hit$kegg,
                    ppm   = hit$ppm,

                    # The libmz is the corresponding m/z value of the
                    # KEGG partner compound.
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
kegg.partners <- function(kegg_id, network) {
    # kegg_id list could be empty???
    is_null <- IsNothing(kegg_id);
    partners <- sapply(network, function(reaction) {
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

    # 20190626 due to the reason of some KEGG reaction its
    # reactome and products are keeps the same
    #
    # filtering such result at here
    yes <- kegg_id != partners;
    partners[yes];
}
