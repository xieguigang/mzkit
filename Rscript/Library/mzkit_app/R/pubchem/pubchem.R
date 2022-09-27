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

#' Query pubchem for knowledge network
#' 
#' @param cid the compound cid 
#'
#' @return a tuple list that contains 3 elements:
#' 
#'   + genes
#'   + disease
#'   + compounds
#' 
const knowledge_graph = function(cid, cache = "./graph_kb") {
    const [genes, disease, compounds] = query.knowlegde_graph(cid, cache = cache);

    {
        "genes": .graph_table(genes, "genes"),
        "disease": .graph_table(disease, "disease"),
        "compounds": .graph_table(compounds, "compounds")
    };
}

const .graph_table = function(nodes, type = ["genes","disease","compounds"]) {
    let u = [{[nodes]::ID_1}]::GraphId;
    let v = [{[nodes]::ID_2}]::GraphId;
    let links = [{[nodes]::Evidence}]::Graph;
    let pubmed = lapply(1:length(u), function(i) {
        .extract_pubmed_evidence(links[i], u[i], v[i]);
    });
    let evidence = NULL;

    for(term in pubmed) {
        evidence = rbind(evidence, term);
    }

    evidence;
}

const .extract_pubmed_evidence = function(evidence, u, v) {
    let Article = [evidence]::Article;
    let pubmed = data.frame(
        GenericArticleId = [Article]::GenericArticleId,
        RelevanceScore = [Article]::RelevanceScore,
        PMID = [Article]::PMID,
        DOI = [Article]::DOI,
        PublicationDate = [Article]::PublicationDate,
        IsReview = [Article]::IsReview,
        Title = [Article]::Title,
        Author = [Article]::Author,
        Journal = [Article]::Journal,
        Citation = [Article]::Citation,
        ChemicalName = [Article]::ChemicalName,
        DiseaseName = [Article]::DiseaseName,
        GeneSymbolName = [Article]::GeneSymbolName
    );

    pubmed[, "u"] = u;
    pubmed[, "v"] = v;
    pubmed[, "NeighborName"] = [evidence]::NeighborName;
    pubmed[, "OrderingByCooccurrenceScore"] = [evidence]::OrderingByCooccurrenceScore;
    pubmed[, "QueryArticleCount"] = [evidence]::QueryArticleCount;
    pubmed[, "NeighborArticleCount"] = [evidence]::NeighborArticleCount;
    pubmed[, "TotalArticleCount"] = [evidence]::TotalArticleCount;
    pubmed[, "EffectiveTotalArticleCount"] = [evidence]::EffectiveTotalArticleCount;
    pubmed[, "ArticleCount"] = [evidence]::ArticleCount;
    pubmed[, "CooccurrenceScore"] = [evidence]::CooccurrenceScore;

    # print(pubmed, max.print = 6);
    pubmed;
}