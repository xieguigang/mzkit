imports "metadna" from "mzkit.insilicons";
imports ["assembly", "data", "math"] from "mzkit";

options(verbose = TRUE);

const input  as string = ?"--mgf"    || stop("A mgf ions file must be provided!");
const output as string = ?"--output" || dirname(input);

const KEGGlibbase = "D:\biodeep\biodeepdb_v3\KEGG";
const KEGGlib = list(
	KEGG_cpd = `${KEGGlibbase}/KEGG_cpd.repo`,
	KEGG_rxn = `${KEGGlibbase}/reaction_class.repo`
);

print("using kegg libraries:");
str(KEGGlib);

const seeds = (function() {
	if (file.exists(?"--seeds")) {
		read.csv(?"--seeds");
	} else {
		NULL;
	}
})();

const metadna = metadna(
	ms1ppm    = tolerance(20, "ppm"),
	dotcutoff = 0.5,
	mzwidth   = tolerance(0.3, "da"),
	allowMs1  = FALSE
)
:> range(["[M]+", "[M+H]+"])
:> load.kegg(kegg.library(repo = KEGGlib$KEGG_cpd))
:> load.kegg_network(kegg.network(repo = KEGGlib$KEGG_rxn))
# :> load.raw(
	# sample = assembly::mzxml.mgf("E:\biodeep\biodeepDB\lxy-CID30.mzML")
# )
;

print("Seeds for the metaDNA infer:");
print(str(seeds));

const infer = metadna :> DIA.infer(
	seeds  = seeds,
	sample = input
		:> read.mgf
		:> mgf.ion_peaks
)
;

metadna 
:> as.table(infer) 
:> write.csv(file = `${output}/${basename(input)}.metaDNA_all.csv`)
;

metadna
:> as.table(infer, unique = TRUE)
:> write.csv(file = `${output}/${basename(input)}.metaDNA.csv`)
;

require(igraph);

metadna
:> as.table(infer)
:> as.graph
:> save.network(file = `${output}/${basename(input)}.metaDNA_infer/`)
;

metadna 
:> as.ticks
:> write.csv(file = `${output}/metaDNA.ticks.csv`)
;