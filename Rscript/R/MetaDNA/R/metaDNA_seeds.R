#Region "Microsoft.ROpen::b665e5ed1750d4b71a9ea2d11fef36b0, metaDNA_seeds.R"

    # Summaries:

    # seeding <- function(output, rt.adjust, seeds.all) {...
    # extends.seeds <- function(output,rt.adjust = function(rt, KEGG_id) 1,seeds.all = TRUE,seeds.topn = 5) {...
    # metaDNA.score <- function(hit) {...
    # extends.seeds.top <- function(seeds, n = 5) {lapply(seeds, function(compound) {	if (length(compound) > n) {...

#End Region

seeding <- function(output, rt.adjust, seeds.all) {	
	seeds <- list();	
	
	for (block in output) {
		if (block %=>% IsNothing) {
			next;
		}
	
		for (seedsCluster in block) {
		
		    if (seedsCluster %=>% IsNothing) {
			    next;
			}
		
			for (feature in seedsCluster) {
				KEGG <- feature$kegg.info$kegg$ID;
				align <- feature$align;
				cluster <- seeds[[KEGG]];
				hit <- list(
					# feature is the current ion ms1 name
					feature   = feature$name,
					spectra   = align$candidate, 
					score     = align$score, 

					# ref and parent is the reference template index
					KEGG      = KEGG,
					ref       = align$ms2.name,
					parent    = align$parent,
					rt.adjust = rt.adjust(feature$feature$rt, KEGG),
					# append the iteration trace stack
					trace     = feature$align$trace
				);							
				hit$trace <- append(hit$trace, hit %=>% trace.node);
				
				# try to fix the duplicated spectra data
				# use this uid tag data
				uid = sprintf("%s#%s", hit$ref$file, hit$ref$scan);
				
				if (cluster %=>% IsNothing) {
					# current feature alignment is the best
					seed <- list();
					seed[[uid]] <- hit;
					seeds[[KEGG]] <- seed;
				} else {
				    if (seeds.all) {
					    # insert all hits as the seeds					
					    cluster[[uid]] <- hit;
						seeds[[KEGG]] <- cluster;
						
						rm(list="cluster");
					} else {
					    # only pick the best hit for seeds
						# due to the reason of only have one best hit record
						# so that we can get the best hit directly by index 1
						best <- cluster[[1]];
						
						if (metaDNA.score(hit) > metaDNA.score(best)) {
							# current feature alignment is the best alignment
							seed <- list();
							seed[[uid]] <- hit;
							seeds[[KEGG]] <- seed;
						} else {
							# no changes
						}
					}					
				}
			}
		}
	}
	
	seeds;
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
#' @param seeds.all If this parameter is enable, then all of the hit from the 
#'       alignment result will be used as metaDNA seeds in the next iteration, 
#'       otherwise the best hit will be picked from the alignment result as 
#'       the seeds if this parameter is set to \code{FALSE}
#'       But if too many seeds in a cluster, then will caused the dataset is too large,
#'       And this will makes the alignment iteration took very long long time for run. 
#'       So just pick the top 5 result when requires all alignment hit as seeds.
#'
extends.seeds <- function(output, 
	rt.adjust  = function(rt, KEGG_id) 1, 
	seeds.all  = TRUE, 
	seeds.topn = 5) {
		
	print("Create metaDNA seeds from alignment result");
	
	# one kegg id have multiple hits or only one best spectra
	seeds <- seeding(output, rt.adjust, seeds.all);
	
	gc();
	
	# and then pick top 5 if use all alignment result hit as seeds
	if (seeds.all && seeds.topn > 0) {
		extends.seeds.top(seeds, n = seeds.topn);
	} else if (seeds.all) {
		# using all hits as seeds
		seeds;
	} else {
		# just best 1
		seeds;
	}
}

#' Seeds score evaluation
#'
#' @param hit The metaDNA alignment hit record 
#'
metaDNA.score <- function(hit) {
	min(hit$score) + hit$rt.adjust;
}

#' Subset of the seeds by top scores
#'
#' @param seeds The seeds data collection
#' @param n top n, by default is top 5
#'
#' @return The subset of the input seeds collection.
#'
extends.seeds.top <- function(seeds, n = 5) {
	lapply(seeds, function(compound) {
		if (length(compound) > n) {
			scores   <- sapply(compound, metaDNA.score) %=>% as.vector;
			desc     <- order(-scores);
			index    <- names(compound)[desc];
			top      <- index[1:n];
			compound <- compound[top];
		}
		
		compound;
	});
}
