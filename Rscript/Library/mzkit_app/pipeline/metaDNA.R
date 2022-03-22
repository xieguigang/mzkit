require(mzkit);

imports "mzweb" from "mzkit";
imports "metadna" from "mzDIA";

# run metaDNA annotation on raw data file

[@info "the LC-MS raw data file for run metaDNA data annotation."]
const raw as string    = ?"--raw"       || stop("no mzXML/mzML/mzPack raw data file is provided!");
[@info "a data directory for output the result plot files."]
const output as string = ?"--outputdir" || `${dirname(raw)}/${basename(raw)}-metadna/`;
const KEGGlib = list(
	KEGG_cpd = system.file("data/KEGG_compounds.msgpack", package = "mzkit"),
	KEGG_rxn = system.file("data/reaction_class.msgpack", package = "mzkit")
);

print("using kegg libraries:");
str(KEGGlib);

sampleData = raw
|> open.mzpack()
|> ms2_peaks()
;

metadna = metadna()
|> metadna::range(["[M]+","[M+H]+","[M+H-H2O]+","[M+H-2H2O]+"])
|> metadna::load.kegg( kegg = kegg.library(repo = KEGGlib$KEGG_cpd) )
|> metadna::load.kegg_network( links = kegg.network(repo = KEGGlib$KEGG_rxn) )
# |> metadna::load.raw( sample = sampleData )
;

infer = metadna |> DIA.infer(sample = sampleData);

metadna 
|> as.table(infer) 
|> write.csv(file = `${output}/metaDNA_all.csv`)
;

metadna
|> as.table(infer, unique = TRUE)
|> write.csv(file = `${output}/metaDNA.csv`)
;

require(igraph);

metadna
|> as.table(infer)
|> as.graph
|> save.network(file = `${output}/metaDNA_infer/`)
;

metadna 
|> as.ticks
|> write.csv(file = `${output}/metaDNA.ticks.csv`)
;