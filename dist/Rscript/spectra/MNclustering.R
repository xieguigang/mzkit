require(biodeepdb_v3);

imports "MoleculeNetworking" from "mzDIA";
imports "mzweb" from "mzkit";
imports "visual" from "mzplot";

raw = [
# "F:\Temp\mzkit_win32\.cache\f2\f2072b661b960c211facfb7a92576c7c.mzPack",
# "F:\Temp\mzkit_win32\.cache\ec\ecd458e1ae2e30a74ccb645d135aae0a.mzPack",
# "F:\Temp\mzkit_win32\.cache\ef\efc7f44675867861835658c83f2b7a40.mzPack"
system.file("data/lxy-CID30.mzPack", package = "biodeepdb_v3")
];# list.files("", pattern = "*.mzPack");

raw = raw 
|> lapply(open.mzpack) 
|> lapply(x -> ms2_peaks(x)) 
|> lapply(uniqueNames) 
|> unlist()
;

print(raw);
print(length(raw));

let network = raw 
|> MoleculeNetworking::clustering(tree.identical = 0.65, tree.right = 0)
;

str(network);

let specturm = network$cluster.raw |> splitClusterRT(rtwin = 60, wrap.peaks = TRUE);

print(specturm);

print("raw data size:");
print(length(raw));
print("cluster size:");
print(length(specturm));

