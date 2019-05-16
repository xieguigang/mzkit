#Region "Microsoft.ROpen::fe49ca3d06fcaed25bcaca2eb725b267, metaDNA_seeds.R"

    # Summaries:

    # extends.seeds <- function(output) {...

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
extends.seeds <- function(output, seeds.all) {
	# one kegg id have multiple hits or only one best spectra
	seeds <- list();
	uid <- 1;
	
	for(block in output) {
		if (block %=>% IsNothing) {
			next;
		}
	
		for (seedsCluster in block) {
			for (feature in seedsCluster) {
				KEGG <- feature$kegg.info$kegg$ID;
				cluster <- seeds[[KEGG]];
				hit <- list(
					spectra = feature$align$candidate,
					score = feature$align$score
				);							
				
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
	
	seeds;
}
