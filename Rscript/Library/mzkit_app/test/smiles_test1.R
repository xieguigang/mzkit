require(mzkit);

imports "SMILES" from "mzkit";

let str = "CC(/C=C/C1=CC=CC=C1)=O.[E]";
let formula = SMILES::parse(str);


