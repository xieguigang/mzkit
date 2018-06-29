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

  match.kegg <- kegg.match.handler(
    meta.KEGG$data,
    unknown$peaktable[, "mz"],
    precursor_type = precursor_type,
	kegg_id        = meta.KEGG$kegg_id,
    tolerance      = tolerance
  );
  lapply(identify$meta.KEGG %=>% .as.list, function(identify) {
  	partners  <- identify$KEGG %=>% kegg.partners;
  	ms2       <- peak_ms2[[identify$peak_ms2.i]];

    metaDNA.impl(partners, ms2, unknown, ms2.align, match.kegg);
  });
}

#' Match unknown by mass
#'
#' @param unknown.mz This mz parameter is a \code{m/z} vector from the \code{unknown$peaktable}
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

  # identify kegg partners => kegg m/z => unknown mz with tolerance => unknown index => unknown peak and ms2 for align
  function(kegg_id) {
	
	# Get kegg m/z for a given kegg_id set
    mzi  <- sapply(kegg.ids, function(id) id %in% kegg_id) %=>% as.logical %=>% which;
    mz   <- kegg.mz[mzi];
	kegg <- kegg.list[mzi];

    sapply(1:length(unknown.mz), function(j) {
		ms1   <- unknown.mz[j];
		query <- sapply(1:length(mz), function(i) {
			mz.x <- mz[i];
			kegg.x <- kegg[i];
			
			if (tolerance(ms1, mz.x)) {
				kegg.x;
			} else {
				NULL;
			}
		});
		
		if (From(query)$Select(IsNothing)$ToArray() %=>% all) {
			query <- NA;
		}
		
		list(unknown.index = j, unknown.mz = ms1, kegg = query);
    });
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
kegg.partner <- function(kegg_id) {
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

#'
#' @param KEGG.partners Related to the identified KEGG id based on the kegg reaction definitions.
#'     Using for find unknown metabolite ms2 data.
#' @param identify.ms2 The identify metabolite's MS/MS matrix data.
#' @param unknown Unknown metabolite's peaktable and peak_ms2 data.
#'
#' @details KEGG.partners -> kegg.match.handler -> unknown index -> unknown ms2 -> identify.ms2 alignment -> is similar?
#'
#'              yes, identify the unknown as KEGG.partner
#'              no, returns NULL
metaDNA.impl <- function(KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches) {
  unknown.i <- KEGG.partners %=>% unknown.matches;
  peaktable <- unknown$peaktable[unknown.i, ];
  peak_ms2  <- unknown$peak_ms2[peaktable[, "peak_ms2.i"]];

}
