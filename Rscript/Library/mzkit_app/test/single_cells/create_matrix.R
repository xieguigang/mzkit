require(mzkit);

imports "SingleCells" from "mzkit";

options(n_threads = 12);

let rawdata = open.mzpack("F:\datafiles\Saccharomyces_cerevisiae.mzPack");
let matrix = cell_matrix(rawdata, mz_matrix = TRUE, mzdiff = 0.01);

write.matrix(matrix, file = "F:\datafiles\Saccharomyces_cerevisiae.dat");