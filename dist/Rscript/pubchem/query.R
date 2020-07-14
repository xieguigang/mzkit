imports "pubchem_kit" from "mzkit";

let input = "D:\biodeep\biodeepdb_v3\Rscript\metadb\bionovogene\bionovogene_flavone.csv";
let info = read.csv(input);
let CAS = info[, "CAS"];

print(CAS);

let result = query(CAS, cache = `${dirname(input)}/bionovogene_flavone`);