require(mzkit);

imports ["MSI", "mzPack", "math"] from "mzkit";
imports "SingleCells" from "mzkit";

let rawdata_matrix = "\\192.168.1.254\backup3\项目以外内容\客户返修项目交付\pig_testis\result_20250718\tmp\workflow_tmp\exportMzMatrix\rawdata.mzImage";
let expr_matrix = rawdata_matrix
|> read.mz_matrix()
|> SingleCells::as.expression()
;