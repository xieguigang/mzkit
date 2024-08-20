require(mzkit);

imports "SMILES" from "mzkit";

let str = "CC(/C=C/C1=CC=CC=C1)=O.CCC.[E]";
let formula = SMILES::parse("[E]"); # SMILES::parse(str);


