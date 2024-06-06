require(mzkit);

imports "SMILES" from "mzkit";

const d_leucine = SMILES::parse("CC(C)C[C@@H](N)C(O)=O");
const valine    = SMILES::parse("CC(C)C(C(=O)O)N");
const l_leucine = SMILES::parse("CC(C)CC(C(=O)O)N");

print(SMILES::links(valine));
print(SMILES::links(l_leucine));
print(SMILES::links(d_leucine));

print(SMILES::score(valine, l_leucine));
print(SMILES::score(valine, d_leucine));

print(SMILES::score(d_leucine, l_leucine));