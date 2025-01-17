require(mzkit);

imports "hmdb_kit" from "mzkit";

let s = read.hmdb("E:\biodeep\hmdb_metabolites.xml");
let tabular_file = "E:\biodeep\hmdb_metabolites.csv";
# generates the in-memory dataset
let msg = s |> export.hmdb_table();

# export to csv file
# s |> export.hmdb_table(file = tabular_file);

writeBin(msg, con = file.path(@dir, "/../data/hmdb.msgpack"));

let table = read.csv(tabular_file, row.names = 1, check.names = FALSE);

table[,"state"]=NULL;
table[,"description"]=NULL;
table[,"secondary_accessions"]=NULL;
table[,"cellular_locations"]=NULL;
table[,"synonyms"]=NULL;
table[,"tissue"]=NULL;
table[,"pathways"]=NULL;
table[,"proteins"]=NULL;
table[,"disease"]=NULL;
table[,"Physiological_effects"]=NULL;
table[,"Disposition"]=NULL;
table[,"Process"]=NULL;
table[,"Role"]=NULL;
table[,"Biomarker"]=NULL;

write.csv(table, file = file.path(@dir, "/../data/hmdb.csv"));