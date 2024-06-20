require(mzkit);

imports "SingleCells" from "mzkit";

options(n_threads = 40);

let rawdata = open.mzpack("Y:\项目以外内容\2024\单细胞代谢分析流程测试\datafiles\bulk_group_2.mzPack");
let matrix = cell_matrix(rawdata, mz_matrix = TRUE, mzdiff = 0.01);

write.matrix(matrix, file = "Y:\项目以外内容\2024\单细胞代谢分析流程测试\datafiles\bulk_group_2.mzImage");