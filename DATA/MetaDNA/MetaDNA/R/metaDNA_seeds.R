#Region "Microsoft.ROpen::92cc22ad8806ffb489ad0a8553eb2725, metaDNA_seeds.R"

    # Summaries:

    # extends.seeds <- function(output, seeds.all) {...

#End Region

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
extends.seeds <- function(output, seeds.all) {
	# one kegg id have multiple hits or only one best spectra
	seeds <- list();	
	
	print("Create metaDNA seeds from alignment result");
	
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
					spectra = align$candidate, 
					score   = align$score, 
					ref     = align$ms2.name
				);							
				
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
						
						if (min(hit$score) > min(best$score)) {
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
	
	gc();
	
	# and then pick top 5 if use all alignment result hit as seeds
	if (seeds.all) {
		seeds %=>% extends.seeds.top;
	} else {
		seeds;
	}
}

extends.seeds.top <- function(seeds, n = 5) {
	lapply(seeds, function(compound) {
		if (length(compound) > n) {
			scores <- sapply(compound, function(m) min(hit$score)) %=>% as.vector;
			desc <- order(-scores);
			index <- names(compound)[desc];
			top <- index[1:n];
			compound <- compound[top];
		}
		
		compound;
	});
}
