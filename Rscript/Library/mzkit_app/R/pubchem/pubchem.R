#' Load mesh background model from run enrichment analysis
#' 
#' @details MeSH (Medical Subject Headings) is the NLM controlled
#'     vocabulary thesaurus used for indexing articles for PubMed.
#' 
const mesh_model = function(topics = NULL) {
    "data/mtrees2022.bin"
    |> system.file(package = "mzkit")
    |> read.mesh_tree()
    |> mesh_background(clusters = topics)
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
        .extract_pubmed_evidence(links[i], u[i], v[i], type);
    });
    let evidence = NULL;

    for(term in pubmed) {
        evidence = rbind(evidence, term);
    }

    evidence;
}

const .term_maps = function(x, type = ["genes","disease","compounds"]) {
	const map = {
		"genes": Article -> list(query = [Article]::ChemicalName, partner = [Article]::GeneSymbolName, type = type),
		"disease": Article -> list(query = [Article]::ChemicalName, partner = [Article]::DiseaseName, type = type),
		"compounds": Article -> list(query = [Article]::ChemicalName_1, partner = [Article]::ChemicalName_2, type = type)
	};
	const f = map[[type]];
	
	f(x);
}

const .extract_pubmed_evidence = function(evidence, u, v, type = ["genes","disease","compounds"]) {
    let Article = [evidence]::Article;
	let nsize = length(Article);
	let na = rep("", nsize);
	let safeProj = function(x) {
		if ([length(x) == 0] || [length(x) != nsize]) {
			na;
		} else {
			x;
		}
	}
	let term = .term_maps(Article, type); # str(term);
    let pubmed = data.frame(
        GenericArticleId = safeProj([Article]::GenericArticleId),
        RelevanceScore = safeProj([Article]::RelevanceScore),
        PMID = safeProj([Article]::PMID),
        DOI = safeProj([Article]::DOI),
        PublicationDate = safeProj([Article]::PublicationDate),
        IsReview = safeProj([Article]::IsReview),
        Title = safeProj([Article]::Title),
        Author = safeProj([Article]::Author),
        Journal = safeProj([Article]::Journal),
        Citation = safeProj([Article]::Citation),
        ChemicalName = term$query,
        partner = term$partner
    );

	if (length(Article) > 0) {
		pubmed[, "type"] = type;
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
	} else {
		NULL;
	}    
}