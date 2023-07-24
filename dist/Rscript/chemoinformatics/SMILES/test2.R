require(mzkit);
imports "SMILES" from "mzkit";
imports "formula" from "mzkit";

# const debug = "C(C=4)(=O)c(c3OC4c(c5)ccc(c5)O)c(O)cc(c3)OC(C(O)1)OC(COC(C2O)OCC2(CO)O)C(O)C1O";
const debug = "CC(C1=CC=C(C=C1)NC(=O)C2CC2)[NH2+]C(C)C(=O)NC3=CC(=C(C=C3)Cl)Cl";
# const debug = "[NH2+]";
const g = SMILES::parse(debug);

print(as.formula(g));
print(SMILES::links(g));