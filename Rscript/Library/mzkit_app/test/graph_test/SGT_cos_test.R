require(mzkit);

imports "SMILES" from "mzkit";

const d_leucine = SMILES::parse("CC(C)C[C@@H](N)C(O)=O", strict = FALSE);
const valine    = SMILES::parse("CC(C)C(C(=O)O)N", strict = FALSE);
const l_leucine = SMILES::parse("CC(C)CC(C(=O)O)N", strict = FALSE);
const salicylic_acid = SMILES::parse("C1=CC=C(C(=C1)C(=O)O)O", strict = FALSE);
const rotenone = SMILES::parse("C12O[C@@]([H])(C(=C)C)CC1=C1O[C@]3([H])COC4C=C(OC)C(OC)=CC=4[C@]3([H])C(=O)C1=CC=2", strict = FALSE);

print(SMILES::links(valine, normalize.size = TRUE));
print(SMILES::links(l_leucine, normalize.size = TRUE));
print(SMILES::links(d_leucine, normalize.size = TRUE));
print(SMILES::links(salicylic_acid, normalize.size = TRUE));
print(SMILES::links(rotenone, normalize.size = TRUE));

print(SMILES::score(valine, l_leucine, normalize.size = TRUE));
print(SMILES::score(valine, d_leucine, normalize.size = TRUE));

print(SMILES::score(d_leucine, l_leucine, normalize.size = TRUE));

print(SMILES::score(salicylic_acid, l_leucine, normalize.size = TRUE));
print(SMILES::score(salicylic_acid, d_leucine, normalize.size = TRUE));
print(SMILES::score(salicylic_acid, valine, normalize.size = TRUE));

print(SMILES::score(salicylic_acid, salicylic_acid, normalize.size = TRUE));
print(SMILES::score(salicylic_acid, rotenone, normalize.size = TRUE));