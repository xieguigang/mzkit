require(mzkit);

imports "hmdb_kit" from "mzkit";

let s = read.hmdb("E:\biodeep\hmdb_metabolites.xml");

s |> export.hmdb_table(file = "E:\biodeep\hmdb_metabolites.csv");