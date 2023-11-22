require(mzkit);

imports ["SingleCells", "MSI"] from "mzkit";

const source = "F:\brain20231113\brain1.mzPack";
const rawdata = open.mzpack(source);
const m = MSI::pixelMatrix(rawdata, mzdiff = "0.01");

setwd(@dir);

write.matrix(m, file = "./brain1.dat");