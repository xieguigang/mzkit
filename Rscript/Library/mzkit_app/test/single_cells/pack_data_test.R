require(mzkit);

let single_cell_source = ["F:\datafiles\bulk_group_1"
"F:\datafiles\bulk_group_2"
"F:\datafiles\bulk_group_3"];

single_cell_source 
|> pack_singleCells(tag = "Saccharomyces_cerevisiae")
|> write.mzPack(file = "F:\\datafiles\Saccharomyces_cerevisiae.mzPack")
;