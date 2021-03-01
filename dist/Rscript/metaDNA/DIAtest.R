imports "metadna" from "mzkit.insilicons";
imports ["assembly", "data", "math"] from "mzkit";

const metadna = metadna(
	ms1ppm    = tolerance(20, "ppm"),
	dotcutoff = 0.5,
	mzwidth   = tolerance(0.3, "da")
)
:> range(["[M]+", "[M+H]+", "[M+H+H2O]+"])
:> load.kegg(kegg.library(repo = "D:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd"))
:> load.kegg_network(kegg.network(repo = "D:\biodeep\biodeepdb_v3\KEGG\reaction_class"))
# :> load.raw(
	# sample = assembly::mzxml.mgf("E:\biodeep\biodeepDB\lxy-CID30.mzML")
# )
;

const rawSample as string = ["F:\111\P201200404.mzXML",
"F:\111\P201200406.mzXML",
"F:\111\P201200407.mzXML",
"F:\111\P201200408.mzXML",
"F:\111\P201200400.mzXML",
"F:\111\P201200401.mzXML",
"F:\111\P201200402.mzXML",
"F:\111\P201200403.mzXML"]; # "E:\biodeep\biodeepDB\lxy-CID30.mzML";
const infer = metadna :> DIA.infer(
	sample = rawSample 
		:> sapply(assembly::mzxml.mgf) 
		:> as.vector
		:> unlist 
		:> centroid 
		:> make.ROI_names
)
;

metadna 
:> as.table(infer) 
:> write.csv(file = `F:/111/all.csv`)
;

metadna
:> as.table(infer, unique = TRUE)
:> write.csv(file = `F:/111/metaDNA.csv`)
;

require(igraph);

metadna
:> as.table(infer)
:> as.graph
:> save.network(file = `F:/111/infers/`)
;