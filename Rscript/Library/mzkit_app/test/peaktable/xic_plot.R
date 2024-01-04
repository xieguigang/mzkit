require(mzkit);

imports "mzDeco" from "mz_quantify";
imports "data" from "mzkit";

options(memory.load = "max");

let table = readBin("D:\\pos.xcms", what = "peak_set");
let XIC = data::XIC(table, mz = 113.1632, tolerance = "da:0.01");

bitmap(file = "D:\\pos_113.1632.png") {
    raw_snapshot3D(XIC);
}

