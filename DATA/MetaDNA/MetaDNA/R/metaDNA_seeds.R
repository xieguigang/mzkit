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
extends.seeds <- function(output) {
	# one kegg id have multiple hits or only one best spectra
	seeds <- list();
	
	for(block in output) {
		if (block %=>% IsNothing) {
			next;
		}
	
		for (seedsCluster in block) {
			for(feature in seedsCluster) {
				KEGG <- feature$kegg.info$kegg$ID;
				best <- seeds[[KEGG]];
				hit <- list(
					spectra = feature$align$candidate,
					score = feature$align$score
				);
				
				if (best %=>% IsNothing) {
					# current feature alignment is the best
					seeds[[KEGG]] <- hit;
				} else {
					if (min(hit$score) > min(best$score)) {
						# current feature alignment is the best alignment
						seeds[[KEGG]] <- hit;
					} else {
						# no changes
					}
				}
			}
		}
	}
	
	seeds;
}
