require(mzkit);

imports ["SingleCells", "MSI"] from "mzkit";

setwd(@dir);

const m = open.matrix("./brain1.dat");
const load = read.mz_matrix(m$reader);