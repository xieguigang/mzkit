require(mzkit);
require(igraph);

imports "SMILES" from "mzkit";

# let smiles_str = "C1C[C@H](O)C(C)(C)[C@]2([H])[C@@H](O[C@H]3[C@H](O)[C@@H](O)[C@H](O)[C@@H](CO)O3)C[C@@]3(C)[C@]4(C)CC[C@@]([C@](O[C@H]5[C@H](O)[C@@H](O)[C@H](O)[C@@H](CO)O5)(C)CC/C=C(\C)/C)([H])[C@@]4([H])[C@H](O)C[C@]3([H])[C@@]12C";
let smiles_str = "Clc1ncc(cc1)CN(C(=NC#N)C)C";
let formula = SMILES::parse(smiles_str);

print(as.formula(formula));