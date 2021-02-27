imports "metadna" from "mzkit.insilicons";
imports ["assembly", "math"] from "mzkit";

metadna(
	ms1ppm    = tolerance(20, "ppm"),
	dotcutoff = 0.5,
	mzwidth   = tolerance(0.3, "da")
)
:> range(["[M]+", "[M+H]+", "[M+H+H2O]+"])
:> load.kegg(kegg.library(repo = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd"))
:> load.kegg_network(kegg.network(repo = "E:\biodeep\biodeepdb_v3\KEGG\reaction_class"))
:> load.raw(
	sample = assembly::mzxml.mgf("E:\biodeep\biodeepDB\lxy-CID30.mzML")
)
:> DIA.infer
;