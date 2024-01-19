require(mzkit);

imports "SingleCells" from "mzkit";

let test_single = read.mz_matrix("E:\etc\test.mzImage");

let df = as.data.frame(test_single);

print(df);