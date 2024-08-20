require(mzkit);

imports "SMILES" from "mzkit";

print(as.data.frame(SMILES::parse("CCCCCC")));
print(as.data.frame(SMILES::parse("cccccc")));
