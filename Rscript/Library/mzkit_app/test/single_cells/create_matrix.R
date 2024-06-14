require(mzkit);

imports "SingleCells" from "mzkit";

options(n_threads = 40);

let rawdata = open.mzpack("E:\biodeep\biodeepdb_v3\datafiles\Saccharomyces_cerevisiae.mzPack");
let matrix = cell_matrix(rawdata, mz_matrix = TRUE, mzdiff = 0.01);

write.matrix(matrix, file = "E:\biodeep\biodeepdb_v3\datafiles\Saccharomyces_cerevisiae.mzImage");