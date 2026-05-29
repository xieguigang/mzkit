require(mzkit);

imports "repository" from "kegg_kit";

print(load.compounds(file("F:\compounds\kegg_metabolites.msgpack"),  rawList = TRUE));