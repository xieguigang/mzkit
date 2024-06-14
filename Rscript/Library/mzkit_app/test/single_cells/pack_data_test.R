require(mzkit);

options(memory.load = "max");

# let single_cell_source = ["E:\biodeep\biodeepdb_v3\datafiles\bulk_group_4"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_5"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_6"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_7"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_8"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_9"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_1"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_2"
# "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_3"];

let single_cell_source = "E:\biodeep\biodeepdb_v3\datafiles\bulk_group_1";

single_cell_source 
|> pack_cells.group(tag = "Saccharomyces_cerevisiae")
|> write.mzPack(file = "E:\biodeep\biodeepdb_v3\datafiles\Saccharomyces_cerevisiae_demo_bulk1.mzPack")
;