require(mzkit);
require(GCModeller);
require(JSON);

#' title: Mummichog peak list annotation
#' author: xieguigang <xie.guigang@gcmodeller.org>
#' description: do ms1 peaks annotation based on the 
#'   Mummichog algorithm.

imports "Mummichog" from "mzkit";
imports "metadna" from "mzDIA";
imports "repository" from "kegg_kit";

setwd(@dir);

# const mzlist = "./test.csv";
const mzlist = "./mz.txt";
const mzdiff = 20;
const minHits = 3;
const permutation = 1000;

const kegg_reactions = GCModeller::kegg_reactions(raw = TRUE);
const kegg_maps      = GCModeller::kegg_maps(rawMaps = TRUE);
const kegg_compounds = GCModeller::kegg_compounds(rawList = TRUE)
|> annotationSet(
    mzdiff     = `ppm:${mzdiff}`,
    precursors = ["[M]+", "[M+H]+", "[M+H-H2O]+"]
)
;

print("query kegg metabolite candidates...");

# get a list of candidate results
# const mzSet = read.csv(mzlist , row.names = 1, check.names = FALSE, check.modes = FALSE)[, "mz"]
const mzSet = mzlist |> readLines()
|> as.numeric()
|> queryCandidateSet(msData = kegg_compounds)
;

print("run data annotations!");

let result = kegg_maps
|> kegg_background(reactions = kegg_reactions)
|> peakList_annotation(
    candidates = mzSet,
    minhit = minHits,
    permutation = permutation,
    ga = FALSE
)
;

print("output results...");

result
|> json_encode()
|> writeLines(con = "./mz_annotations.json")
;

result = as.data.frame(result);

print("total activity enrichment score is:");
print(sum(result[, "activity"]));

print("view of the annotation result output:");
print(result, max.print = 13, select = ["description","Q","input_size","background_size","activity","p-value"]);

write.csv(result, file = "./mz_annotations.csv");