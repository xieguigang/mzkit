require(mzkit);

imports "lipidomics" from "mzDIA";

let lipid = lipidomics::parse_lipid("PC 14:1_18:0");

str(as.list(lipid));
print(lipid_smiles(lipid));