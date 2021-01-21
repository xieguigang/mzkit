imports "pubchem_kit" from "mzkit";

let kegg = as.data.frame(SID_map("F:\pubchem\SID-Map.KEGG.txt", dbfilter = "KEGG"));

str(kegg);

print(kegg$registryIdentifier)