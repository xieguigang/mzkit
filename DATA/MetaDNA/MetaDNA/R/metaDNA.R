require(VisualBasic.R);

Imports("Microsoft.VisualBasic.Data");
Imports("Microsoft.VisualBasic.Data.Linq");
Imports("Microsoft.VisualBasic.Language");

# KEGG注释原理：假设在某一些特定类型的代谢过程而言，其反应底物和反应产物的区别仅仅在于一些基团的加减，
# 则二级结构应该是比较相似的
# 故而，对于一个反应过程 A <=> B ，假若数据库之中没有找B的二级数据，但是却找到了A的二级数据，
# 那么就可以通过这个原理来进行B的注释

# 算法计算过程
#
# 1. 在通过SSM鉴定之后，大致可以依据二级结果的相似度将sample分为已鉴定代谢物和未鉴定代谢物
# 2. 对未鉴定代谢物进行遍历，通过未鉴定的代谢物的mz进行KEGG代谢物的一级查找，找出所有的可能结果
# 3. 将查找到的KEGG编号从已鉴定代谢物之中取补集，即取出已鉴定代谢物之中不存在的KEGG编号
# 4. 利用KEGG代谢反应过程找出和未鉴定代谢物的KEGG编号相匹配的同过程内的KEGG代谢物对应的已鉴定代谢物的二级质谱信息
# 5. 进行二级比较，如果二级相似度较高，则确认该未鉴定代谢物可能为某一个KEGG编号


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
#' @param tolerance m/z equalient compares tolerance, by default is less than ppm 20.
#'
#' @param score.cutoff MS/MS similarity cutoff for identify ms2 alignment with unknown ms2
#'
#' @param ms2.align The MS/MS alignment method, which is in format like: \code{function(q, s)}
#'      Where \code{q} and \code{s} is a matrix.
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
metaDNA <- function(identify, unknown, meta.KEGG, ms2.align,
                    precursor_type = "[M+H]+",
                    tolerance      = tolerance.ppm(20),
                    score.cutoff   = 0.8) {

  # 1. Find all of the related KEGG compound by KEGG reaction link for
  #    identify metabolites
  # 2. Search for unknown by using ms1 precursor_m/z compare with the
  #    KEGG compound molecule weight in a given precursor_type mode.

  # load kegg reaction database
  # data/metaDNA_kegg.rda
  xLoad("metaDNA_kegg.rda");

  kegg_id.col <- meta.KEGG$kegg_id;
  meta.KEGG   <- meta.KEGG$data;
  match.kegg  <- kegg.match.handler(
    meta.KEGG,
    unknown$peaktable[, "mz"],
    precursor_type = precursor_type,
	  kegg_id        = kegg_id.col,
    tolerance      = tolerance
  );
  identify.peak_ms2 <- identify$peak_ms2;

  # tick.each
  # lapply
  tick.each(identify$meta.KEGG %=>% .as.list, function(identified) {
  	partners  <- identified$KEGG %=>% kegg.partners;
  	ms2       <- identify.peak_ms2[[identified$peak_ms2.i]];

  	# current identify metabolite KEGG id didnt found any
  	# reaction related partner compounds
  	# Skip current identify metabolite.
  	if (IsNothing(partners)) {
  		return(NULL);
  	}

  	# KEGG.partners, identify.ms2, unknown, ms2.align, unknow.matches
    metaDNA.impl(
  		KEGG.partners  = partners,
  		identify.ms2   = ms2,
  		unknown        = unknown,
  		ms2.align      = ms2.align,
  		unknow.matches = match.kegg,
  		score.cutoff   = score.cutoff
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

  kegg.mass <- meta.KEGG[,  "exact_mass"] %=>% as.numeric;
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
    # Get corresponding kegg mz and annotation meta data
    mz   <- kegg.mz[mzi];
	  kegg <- kegg.list[mzi];

	  # Loop on each unknown metabolite ms1 m/z
	  # And using this m/z for match kegg m/z to
	  # get possible kegg meta annotation data.
    unknown.query <- sapply(1:length(unknown.mz), function(j) {
  		ms1   <- unknown.mz[j];
  		query <- sapply(1:length(mz), function(i) {
        # unknown metabolite ms1 m/z match
  		  # kegg mz with a given tolerance
    		if (tolerance(ms1, mz[i])) {
    		  # If these two m/z value meet the tolerance condition
    		  # then we match a possible KEGG annotation data.
    		  # also returns with ppm value
    			list(kegg = kegg[i], ppm = PPM(ms1, mz[i]));
    		} else {
    			NULL;
    		}
  		});

  		# removes all null result
  		nulls <- sapply(query, is.null) %=>% unlist;
  		query <- query[!nulls];

  		if (length(query) == 0) {
  			return(NULL);
  		}

  		if (length(query) > 1 && is.null(names(query))) {
  		  # returns with min ppm?
  		  min_ppm.i <- tryCatch({
  		     sapply(query, function(q) q$ppm) %=>% which.min;

  		  }, error = function(e) {
  		    print(query);
  		    stop(e);
  		  })

  		  query     <- query[min_ppm.i];
  		  query     <- query[[1]];
  		} else {

  		}

  		# print(query);

			list(
			   unknown.index = j,
				 unknown.mz    = ms1,
				 # current unknown metabolite could have
				 # multiple kegg annotation result, based on the ms1
				 # tolerance.
				 kegg = query$kegg,
				 ppm  = query$ppm
			);

    });

    # removes null result
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
  unknown.query <- KEGG.partners %=>% unknow.matches;

  if (IsNothing(unknown.query)) {
	  return(NULL);
  }

  # unknown.i integer index of the peaktable
  unknown.i <- sapply(unknown.query, function(x) x$unknown.index) %=>% unlist;
  # subset of the peaktable by using the unknown index value
  peaktable <- ensure.dataframe(
  	unknown$peaktable[unknown.i, ],
  	colnames(unknown$peaktable)
  );
  # rownames of peaktable is the list names for the peak_ms2
  peak_ms2.index <- peaktable %=>% rownames;

  # peak_ms2 and peaktable is corresponding to each other
  peak_ms2       <- unknown$peak_ms2[peak_ms2.index];
  peaktable      <- peaktable %=>% .as.list;

  # alignment of the ms2 between the identify and unknown
  # The unknown will identified as identify.ms2 when ms2.align
  # pass the threshold cutoff.
  query.result <- list();

  # loop on current unknown match list from the identify kegg partners
  for (i in 1:length(peak_ms2.index)) {
  	# identify for each unknown metabolite
	  kegg.query  <- unknown.query[i];
  	name        <- peak_ms2.index[i];
  	peak        <- peak_ms2[[name]];
  	ms1.feature <- peaktable[[i]];
    best.score  <- -10000;
    best        <- NULL;

    # Loop each identify and using its ms2 as reference
  	for(fileName in names(identify.ms2)) {

  	  file <- identify.ms2[[fileName]];

  	  for (scan in names(file)) {
  	    result <- align_best.internal(
  	      ref          = file[[scan]],
  	      peak         = peak,
  	      ms2.align    = ms2.align,
  	      score.cutoff = score.cutoff
  	    );

        if (!is.null(result)) {
          if (mean(result$score) > best.score) {
            best.score <- mean(result$score);
            best <- result;
          }
        }
  	  }
  	}

    if (!is.null(best)) {
      # name is the peaktable rownames
      query.result[[name]] <- list(
        ms2.alignment = best,
        ms1.feature   = ms1.feature,
		    kegg.info     = kegg.query
      );
    }
  }

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
  score      <- c();
  candidate  <- NULL;

  colnames(ref) <- c("ProductMz", "LibraryIntensity");

  # loop each unknown for alignment best result
  for (fileName in names(peak)) {
    file <- peak[[fileName]];

    for (scan in names(file)) {
      unknown      <- file[[scan]];
      align.scores <- ms2.align(unknown, ref);

      if (mean(align.scores) > best.score) {
        best.score <- mean(align.scores);
        score      <- align.scores;
        candidate  <- unknown;
      }
    }
  }

  if (!IsNothing(score) && all(score >= score.cutoff)) {
    list(ref       = ref,
         candidate = candidate,
         score     = score
    );
  } else {
    NULL;
  }
}

