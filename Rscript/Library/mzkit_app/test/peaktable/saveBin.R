require(mzkit);

imports "mzDeco" from "mz_quantify";

options(memory.load = "max");

let table = read.xcms_peaks("D:\\pos.csv");

writeBin(table, con = "D:\\pos.xcms");

table = readBin("D:\\pos.xcms", what = "peak_set");