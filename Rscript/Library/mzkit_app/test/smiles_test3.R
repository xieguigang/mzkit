require(mzkit);

imports "SMILES" from "mzkit";

let formula = SMILES::parse("CC1=CC=CC=C1");

print(formula);