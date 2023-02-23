require(mzkit);

imports "metadna" from "mzDIA";

["D:\biodeep\biodeepdb_v3\KEGG\reaction_class"]
:> reaction_class.table
:> write.csv(file = "D:\biodeep\biodeepdb_v3\KEGG\reactionclass_table.csv")
;