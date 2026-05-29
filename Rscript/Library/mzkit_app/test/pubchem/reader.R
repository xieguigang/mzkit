require(mzkit);

imports ["pubchem_kit" "massbank"] from "mzkit";

print(readBin("F:\compounds\hmdb_metabolites.msgpack", what = "metalib"));