# require(mzkit);

imports "formula" from "mzkit";

setwd(@dir);

# let simple = formula::parse_SMILES("O");
let simple = formula::parse_SMILES("C=C(O=CCCC)CC");


print(as.formula(simple));

let A = formula::parse_SMILES(readText("test_smiles_graphs\A.smi"), strict = FALSE);
let B = formula::parse_SMILES(readText("test_smiles_graphs\B.smi"), strict = FALSE);
let C = formula::parse_SMILES(readText("test_smiles_graphs\C.smi"), strict = FALSE);

print("molecule A:");
print(as.data.frame(A));
print("molecule B:");
print(as.data.frame(B));
print("molecule C:");
print(as.data.frame(C));