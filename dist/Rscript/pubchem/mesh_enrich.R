require(mzkit);
require(GCModeller);

imports "GSEA" from "gseakit";

setwd(@dir);

terms = read.csv("./annotationResult.csv")[, "MeSH"];
terms = unique(terms);

mesh = mzkit::mesh_model();
mesh 
|> enrichment(
	terms, outputAll = FALSE
)
|> enrichment.FDR()
|> as.data.frame()
|> write.csv("./mesh.csv", row.names = TRUE)
;
