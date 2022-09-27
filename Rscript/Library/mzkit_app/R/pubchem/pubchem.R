#' Load mesh background model from run enrichment analysis
#' 
#' @details MeSH (Medical Subject Headings) is the NLM controlled
#'     vocabulary thesaurus used for indexing articles for PubMed.
#' 
const mesh_model = function() {
    "data/mtrees2022.bin"
    |> system.file(package = "mzkit")
    |> read.mesh_tree()
    |> mesh_background()
    ;
}

