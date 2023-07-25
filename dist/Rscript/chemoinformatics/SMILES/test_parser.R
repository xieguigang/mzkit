require(mzkit);
imports "SMILES" from "mzkit";
imports "formula" from "mzkit";

# C3H10N2
# print(toString(as.formula(SMILES::parse("NCCCN", strict = FALSE))));

# C4H6O3
# print(toString(as.formula(SMILES::parse("CCC(=O)C(O)=O", strict = FALSE))));

# C4H8O3
# print(toString(as.formula(SMILES::parse("[H]OC(=O)[C@@]([H])(O[H])C([H])([H])C([H])([H])[H]", strict = FALSE))));

# C19H24O3
print(toString(as.formula(SMILES::parse("[H][C@@]12CCC(=O)[C@@]1(C)CC[C@]1([H])C3=C(CC[C@@]21[H])C=C(O)C(OC)=C3", strict = FALSE))));

# C7H11N3O2
print(toString(as.formula(SMILES::parse("CN1C=NC(C[C@H](N)C(O)=O)=C1", strict = FALSE))));