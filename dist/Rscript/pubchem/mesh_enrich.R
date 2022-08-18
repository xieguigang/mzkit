require(mzkit);
require(GCModeller);

imports "GSEA" from "gseakit";

setwd(@dir);

terms = read.csv("./annotationResult.csv")[, "MeSH"];
terms = unique(terms);

mesh = mzkit::mesh_model();
mesh = mesh 
|> enrichment(
	terms, outputAll = FALSE
)
|> enrichment.FDR()
|> as.data.frame()
;

mesh[, "name"] = NULL;
mesh[, "description"] = NULL;
mesh[, "cluster"] = NULL;
mesh[, "enriched"] = NULL;
mesh = mesh |> rename(
	geneIDs -> mesh_terms
);

print(mesh, max.print = 13);

mesh
|> write.csv("./mesh.csv", row.names = TRUE)
;
