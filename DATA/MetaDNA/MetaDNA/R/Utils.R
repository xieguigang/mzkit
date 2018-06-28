removes.empty_KEGG <- function(d) {
	KEGG <- as.vector(d[, "KEGG"]);
	KEGG <- Strings.Empty(KEGG, TRUE);
	
	d[!KEGG, ];
}