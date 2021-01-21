imports "pubchem_kit" from "mzkit";

let kegg = as.data.frame(SID_map("F:\pubchem\SID-Map.KEGG.txt", dbfilter = "KEGG"));

str(kegg);

print(unique(kegg$registryIdentifier));


for (cid in unique(kegg$CID)) {

}