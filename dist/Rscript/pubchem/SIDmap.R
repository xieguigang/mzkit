imports "pubchem_kit" from "mzkit";

print(as.data.frame(SID_map("F:\pubchem\SID-Map.KEGG.txt", dbfilter = "KEGG")))