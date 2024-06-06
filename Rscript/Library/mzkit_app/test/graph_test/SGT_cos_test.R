require(mzkit);

imports "SMILES" from "mzkit";

const d_leucine = SMILES::parse("CC(C)C[C@@H](N)C(O)=O");
const valine    = SMILES::parse("CC(C)C(C(=O)O)N");
const l_leucine = SMILES::parse("CC(C)CC(C(=O)O)N");

print(valine);
print(l_leucine);
print(d_leucine);

