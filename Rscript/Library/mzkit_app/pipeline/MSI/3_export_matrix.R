require(mzkit);

imports "MSI" from "mzkit";
imports "math" from "mzkit";
imports "SingleCells" from "mzkit";

let rawdata = open.mzpack("D:/demo/test.mzPack");
let ionsList = load.csv("D:/demo/test_ions.csv", type = "mass_window");
let matrix = rawdata |> peakMatrix(mzError = "da:0.01", ionSet = ionsList, raw_matrix = TRUE);

SingleCells::write.matrix(matrix, file = "D:/demo/test.csv");