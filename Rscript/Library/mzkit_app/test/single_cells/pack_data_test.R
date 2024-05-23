require(mzkit);

let single_cell_source = "F:\\Escherichia_coli";

single_cell_source 
|> pack_singleCells(rawdata, tag = "Escherichia_coli")
|> write.mzPack(file = "F:\\Escherichia_coli.mzPack")
;