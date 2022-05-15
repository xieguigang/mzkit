require(mzkit);
require(GCModeller);

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

const kegg_reactions = GCModeller::kegg_reactions(raw = TRUE);
const kegg_maps      = GCModeller::kegg_maps(rawMaps = TRUE);
const kegg_compounds = GCModeller::kegg_compounds(rawList = TRUE)
|> annotationSet(
    mzdiff     = `ppm:${mzdiff}`,
    precursors = ["[M]+", "[M+H]+", "[M+H-H2O]+"]
)
;

# get a list of candidate results
const mzSet = mzlist 
|> readLines()
|> as.numeric()
|> queryCandidateSet(msData = kegg_compounds)
;

kegg_maps
|> kegg_background(reactions = kegg_reactions)
|> peakList_annotation(
    candidates = mzSet,
    minhit = minHits,
    permutation = permutation
)
;
