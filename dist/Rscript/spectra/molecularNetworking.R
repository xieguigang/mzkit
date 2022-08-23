require(mzkit);
require(igraph);

imports "MoleculeNetworking" from "mzDIA";
imports "mzweb" from "mzkit";
imports "visual" from "mzplot";

raw = [
# "F:\Temp\mzkit_win32\.cache\f2\f2072b661b960c211facfb7a92576c7c.mzPack",
# "F:\Temp\mzkit_win32\.cache\ec\ecd458e1ae2e30a74ccb645d135aae0a.mzPack",
# "F:\Temp\mzkit_win32\.cache\ef\efc7f44675867861835658c83f2b7a40.mzPack"
"D:\biodeep\biodeep_pipeline\biodeepMSMS_v5\test\lxy-CID30\tmp\.cache\raw\9f\lxy-CID30.mzPack"
];# list.files("", pattern = "*.mzPack");

raw = raw 
|> lapply(open.mzpack) 
|> lapply(ms2_peaks) 
|> lapply(uniqueNames) 
|> unlist()
;

network = raw 
|> MoleculeNetworking::tree()
|> as.graph()
|> louvain_cluster()
;

bitmap(file = `${@dir}/hist.png`) {
	plotNetworkClusterHistogram(network);
}