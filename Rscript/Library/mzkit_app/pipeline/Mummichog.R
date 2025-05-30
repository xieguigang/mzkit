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

[@info "A text file that contains a list of the MS1 
        m/z peaks to run data annotations, the text 
        file format should be multiple lines data, 
        and each line text is the m/z value of the 
        given ions list."]
[@type "*.txt"]
const mzlist as string = ?"--mzlist" || stop("A set of m/z value list must be provided!");
[@info "A result table for export of the m/z peak list 
        data annotation results."]
[@type "*.csv"]
const output as string = ?"--save" || `${dirname(mzlist)}/${basename(mzlist)}_annotations.csv`;
[@info "the min number hits of metabolites in a pathway
        for keeps as the annotation result data."]
const minHits as integer = ?"--minhits" || 3;
[@info "the number of times to run annotation permutation."]
const permutation as integer = ?"--permutation" || 100;
[@info "the max ppm tolerance error between two m/z 
        value for run ms1 candidate search 
        annotation."]
const mzdiff as double = ?"--mzdiff" || 20;

if(!file.exists(mzlist)) {
    stop(`the given m/z peak list input data file is not found on your filesystem location: '${mzlist}'!`);
}

print("loading of the KEGG data repository for run data annotations!");

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
const mzSet = mzlist 
|> readLines()
|> as.numeric()
|> take(20000)
|> queryCandidateSet(msData = kegg_compounds)
;

print("run data annotations!");

result = kegg_maps
|> kegg_background(reactions = kegg_reactions)
|> peakList_annotation(
    candidates = mzSet,
    minhit = minHits,
    permutation = permutation
)
;

print("output results...");

result
|> json_encode()
|> writeLines(con = `${dirname(output)}/${basename(output)}.json`)
;

result = as.data.frame(result);

print("total activity enrichment score is:");
print(sum(result[, "activity"]));

print("view of the annotation result output:");
print(result, max.print = 13, select = ["description","Q","input_size","background_size","activity","p-value"]);

write.csv(result, file = output);