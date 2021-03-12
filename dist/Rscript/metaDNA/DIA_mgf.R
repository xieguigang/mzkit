imports "metadna" from "mzkit.insilicons";
imports ["assembly", "data", "math"] from "mzkit";

setwd(dirname(@script));

const metadna = metadna(
	ms1ppm    = tolerance(20, "ppm"),
	dotcutoff = 0.5,
	mzwidth   = tolerance(0.3, "da")
)
:> range(["[M]+", "[M+H]+", "[M+H+H2O]+"])
:> load.kegg(kegg.library(repo = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo"))
:> load.kegg_network(kegg.network(repo = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo"))
# :> load.raw(
	# sample = assembly::mzxml.mgf("E:\biodeep\biodeepDB\lxy-CID30.mzML")
# )
;

const rawSample as string = "E:\biodeep\raw.mgf";
const infer = metadna :> DIA.infer(
	sample = rawSample 
		:> read.mgf
		:> mgf.ion_peaks
)
;

metadna 
:> as.table(infer) 
:> write.csv(file = "E:\biodeep\raw.csv")
;

metadna
:> as.table(infer, unique = TRUE)
:> write.csv(file = "E:\biodeep\raw_metaDNA.csv")
;

require(igraph);

metadna
:> as.table(infer)
:> as.graph
:> save.network(file = "E:\biodeep\inferNetwork")
;