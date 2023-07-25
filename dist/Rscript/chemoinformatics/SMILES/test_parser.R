require(mzkit);
imports "SMILES" from "mzkit";
imports "formula" from "mzkit";

# C7H11N3O2
print(toString(as.formula(SMILES::parse("CN1C=NC(C[C@H](N)C(O)=O)=C1", strict = FALSE))));