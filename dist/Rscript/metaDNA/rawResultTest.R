imports "metadna" from "mzkit.insilicons";
imports "mzweb" from "mzkit";
imports ["assembly", "data", "math"] from "mzkit";

options(verbose = TRUE);

const input as string    = ?"--mzpack"  || stop("A mzpack ions file must be provided!");
const output as string   = ?"--output"  || dirname(input);
const ionMode as integer = ?"--ionMode" || 1;

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

let getIonRange as function() {
	if (ionMode == 1) {
		["[M]+", "[M+H]+"];
	} else {
		["[M]-", "[M-H]-"];
	}
}

const metadna = metadna(
	ms1ppm    = tolerance(20, "ppm"),
	dotcutoff = 0.5,
	mzwidth   = tolerance(0.3, "da"),
	allowMs1  = FALSE
)
:> range(getIonRange())
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
		:> open.mzpack
		:> ms2_peaks
)
;

metadna 
:> as.table(infer) 
:> write.csv(file = `${output}/metaDNA_all.csv`)
;

const unique_result = metadna :> as.table(infer, unique = TRUE);

unique_result
:> write.csv(file = `${output}/metaDNA.csv`)
;

metadna::result.alignment(infer, unique_result)
:> xml
:> writeLines(con = `${output}/metaDNA_infer.XML`)
;

require(igraph);

metadna
:> as.table(infer)
:> as.graph
:> save.network(file = `${output}/metaDNA_infer/`)
;

metadna 
:> as.ticks
:> write.csv(file = `${output}/metaDNA.ticks.csv`)
;