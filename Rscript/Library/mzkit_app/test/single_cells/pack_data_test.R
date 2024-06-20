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

let single_cell_source = "Y:\项目以外内容\2024\单细胞代谢分析流程测试\datafiles\bulk_group_2";

single_cell_source 
|> pack_cells.group(tag = "Saccharomyces_cerevisiae")
|> write.mzPack(file = "Y:\项目以外内容\2024\单细胞代谢分析流程测试\datafiles\bulk_group_2.mzPack")
;