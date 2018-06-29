require(VisualBasic.R);

Imports("Microsoft.VisualBasic.Data");
Imports("Microsoft.VisualBasic.Data.Linq");
Imports("Microsoft.VisualBasic.Language");

#' Identify unknown metabolite by metaDNA algorithm
#'
#' @description How to: The basic idea of the \code{MetaDNA} algorightm is
#'      using the identified ms2 data to align the unknown ms2 data.
#'
#' @param identify A \code{list} object with two members:
#'
#'      \code{identify = list((data.frame) meta.KEGG, (list) peak_ms2)}
#'
#'      where:
#'
#'      \enumerate{
#'         \item \code{meta.KEGG} is a data.frame object that should contains
#'               a KEGG id column, and a index column to read \code{peak_ms2} list.
#'         \item \code{peak_ms2} is a MS/MS list, MS/MS matrix should contains at
#'               least two column: \code{mz} and \code{into}
#'      }
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
#' @return A \code{identify} parameter data structure like metabolite identify
#'      result for \code{unknown} parameter input data
#'
metaDNA <- function(identify, unknown, meta.KEGG, ms2.align,
                    precursor_type = "[M+H]+",
                    tolerance = tolerance.ppm(20)) {

  # 1. Find all of the related KEGG compound by KEGG reaction link for
  #    identify metabolites
  # 2. Search for unknown by using ms1 precursor_m/z compare with the
  #    KEGG compound molecule weight in a given precursor_type mode.

  # load kegg reaction database
  # data/metaDNA_kegg.rda
  xLoad("metaDNA_kegg.rda");

  kegg_id.col <- meta.KEGG$kegg_id;
  meta.KEGG <- meta.KEGG$data;
  match.kegg <- kegg.match.handler(
    meta.KEGG,
    unknown$peaktable[, "mz"],
    precursor_type = precursor_type,
	kegg_id        = kegg_id.col,
    tolerance      = tolerance
  );
  identify.peak_ms2 <- identify$peak_ms2;

  lapply(identify$meta.KEGG %=>% .as.list, function(identified) {
  	partners  <- identified$KEGG %=>% kegg.partners;
  	ms2       <- identify.peak_ms2[[identified$peak_ms2.i]];

	# KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches
    metaDNA.impl(
		KEGG.partners  = partners,
		identify.ms2   = ms2,
		unknown        = unknown,
		ms2.align      = ms2.align,
		unknow.matches = match.kegg
	);
  });
}

#' Match unknown by mass
#'
#' @param unknown.mz This mz parameter is a \code{m/z} vector from the
#'         \code{unknown$peaktable}
#'
#' @return Returns the index vector in \code{unknown.mz} vector.
kegg.match.handler <- function(meta.KEGG, unknown.mz,
                               precursor_type = "[M+H]+",
							   kegg_id        = "KEGG",
                               tolerance      = tolerance.ppm(20)) {

  kegg.mass <- meta.KEGG[,  "mass"] %=>% as.numeric;
  kegg.ids  <- meta.KEGG[, kegg_id] %=>% as.character;
  kegg.mz   <- get.PrecursorMZ(kegg.mass, precursor_type);
  kegg.list <- meta.KEGG %=>% .as.list;

  # identify kegg partners
  #   => kegg m/z
  #   => unknown mz with tolerance
  #   => unknown index
  #   => unknown peak and ms2 for align
  function(kegg_id) {

	# Get kegg m/z for a given kegg_id set
    mzi  <- sapply(kegg.ids, function(id) {
		id %in% kegg_id;
	}) %=>% as.logical %=>% which;
    mz   <- kegg.mz[mzi];
	kegg <- kegg.list[mzi];

    unknown.query <- sapply(1:length(unknown.mz), function(j) {
		ms1   <- unknown.mz[j];
		query <- sapply(1:length(mz), function(i) {
			if (tolerance(ms1, mz[i])) {
				kegg[i];
			} else {
				NULL;
			}
		});
		nulls <- sapply(query, is.null) %=>% unlist;
		query <- query[!nulls];

		if (length(query) == 0) {
			NULL;
		} else {
			list(unknown.index = j,
				 unknown.mz    = ms1,
				 kegg          = query
			);
		}
    });

	nulls <- sapply(unknown.query, is.null) %=>% unlist;
	unknown.query[!nulls];
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
kegg.partners <- function(kegg_id) {
  sapply(network, function(reaction) {
    if (kegg_id %in% reaction$reactants) {
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
#' @details Algorithm routine:
#'
#'    \code{KEGG.partners -> kegg.match.handler
#'                        -> unknown index
#'                        -> unknown ms2
#'                        -> identify.ms2 alignment
#'                        -> is similar?
#'    }
#'
#'    \enumerate{
#'        \item yes, identify the unknown as \code{\link{kegg.partners}}
#'        \item no, returns \code{NULL}
#'    }
#'
metaDNA.impl <- function(KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches) {
  unknown.query <- KEGG.partners %=>% unknow.matches;
  unknown.i <- sapply(unknown.query, function(x) x$unknown.index) %=>% unlist;
  peaktable <- unknown$peaktable[unknown.i, ];
  peak_ms2  <- unknown$peak_ms2[peaktable %=>% rownames];

  # alignment of the ms2 between the identify and unknown
  # The unknown will identified as identify.ms2 when ms2.align
  # pass the threshold cutoff.

}
