imports "metadna" from "mzkit.insilicons";
imports ["assembly", "data", "math"] from "mzkit";

setwd(dirname(@script));

const input as string    = ?"--mgf"     || stop("A mgf ions file must be provided!");
const output as string   = ?"--output"  || dirname(input);
const ionMode as integer = ?"--ionMode" || 1;

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
:> load.kegg(kegg.library(repo = "D:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo"))
:> load.kegg_network(kegg.network(repo = "D:\biodeep\biodeepdb_v3\KEGG\reaction_class.repo"))
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