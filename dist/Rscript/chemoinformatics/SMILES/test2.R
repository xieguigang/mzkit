require(mzkit);
imports "SMILES" from "mzkit";
imports "formula" from "mzkit";

# const debug = "C(C=4)(=O)c(c3OC4c(c5)ccc(c5)O)c(O)cc(c3)OC(C(O)1)OC(COC(C2O)OCC2(CO)O)C(O)C1O";
# const debug = "CC(C1=CC=C(C=C1)NC(=O)C2CC2)[NH2+]C(C)C(=O)NC3=CC(=C(C=C3)Cl)Cl";
# const debug = "C(OC(C(O)6)OC(C(O)C6OC(c(c7)cccc7)=O)CO)(C1OC(C4=O)=C(c(c5)ccc(c5O)O)Oc(c24)cc(OC(C(O)3)OC(C)C(O)C3O)cc2O)C(O)C(O)C(O1)CO";
# const debug = "[NH2+]";
const debug = "C(=CC(OCC(O2)C(C(C(C2OC(C(Oc(c6)c(c(c7)ccc(O)c7)[o+1]c(c64)cc(cc4OC(C(O)5)OC(CO)C(O)C(O)5)O)3)C(C(C(CO)O3)O)O)O)O)O)=O)c(c1)ccc(O)c(OC)1";
const g = SMILES::parse(debug, strict = TRUE);

print(as.formula(g));
print(SMILES::links(g));