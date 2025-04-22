require(mzkit);

imports "lipidomics" from "mzDIA";

print(as.list(lipidomics::parse_lipid("PC(14:1COOH_18:0)")));