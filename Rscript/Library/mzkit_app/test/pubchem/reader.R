require(mzkit);

imports ["pubchem_kit" "massbank"] from "mzkit";

print(readBin("F:\compounds\kegg_metabolites.msgpack", what = "metalib"));