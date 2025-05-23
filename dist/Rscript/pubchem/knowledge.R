require(mzkit);

imports "pubchem_kit" from "mzkit";

setwd(@dir);

const [genes, disease, compounds] = knowledge_graph("2244");

#str(genes);

# pubmed = [genes]::Evidence;
# pubmed = [pubmed]::ChemicalGeneSymbolNeighbor;
# pubmed = unlist([pubmed]::Article);

# pubmed = data.frame(

# GenericArticleId = [pubmed]::GenericArticleId,
  # RelevanceScore = [pubmed]::RelevanceScore,
  # PMID  = [pubmed]::PMID,
  # DOI  = [pubmed]::DOI,
  # PublicationDate  = [pubmed]::PublicationDate,
  # IsReview  = [pubmed]::IsReview,
  # Title  = [pubmed]::Title,
  # Author  = [pubmed]::Author,
  # Journal  = [pubmed]::Journal ,
  # Citation  = [pubmed]::Citation,
  # ChemicalName  = [pubmed]::ChemicalName,
  # DiseaseName  = [pubmed]::DiseaseName,
  # GeneSymbolName  = [pubmed]::GeneSymbolName 

# );

# print(pubmed);

print(genes);
print(disease);
print(compounds);

write.csv(genes, file = "./2244_genes.csv", row.names = FALSE);
write.csv(disease, file = "./2244_disease.csv", row.names = FALSE);
write.csv(compounds, file = "./2244_compounds.csv", row.names = FALSE);