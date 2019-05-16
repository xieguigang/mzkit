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
#'
extends.seeds <- function(output, seeds.all) {
	# one kegg id have multiple hits or only one best spectra
	seeds <- list();
	uid <- 0;
	
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
				hit <- list(spectra = align$candidate, score = align$score);							
				
				uid = uid + 1;
				
				if (cluster %=>% IsNothing) {
					# current feature alignment is the best
					seed <- list();
					seed[[sprintf("#%s", uid)]] <- hit;
					seeds[[KEGG]] <- seed;
				} else {
				    if (seeds.all) {
					    # insert all hits as the seeds
					    cluster[[sprintf("#%s", uid)]] <- hit;
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
							seed[[sprintf("#%s", uid)]] <- hit;
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
	
	seeds;
}
