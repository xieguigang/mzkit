# require(mzkit);

imports "formula" from "mzkit";

setwd(@dir);

let A = formula::parse_SMILES(readText("test_smiles_graphs\A.smi"));
let B = formula::parse_SMILES(readText("test_smiles_graphs\B.smi"));
let C = formula::parse_SMILES(readText("test_smiles_graphs\C.smi"));

