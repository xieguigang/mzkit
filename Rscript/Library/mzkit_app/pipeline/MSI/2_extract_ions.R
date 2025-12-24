require(mzkit);

imports "MSI" from "mzkit";
imports "mzweb" from "mzkit";

let rawdata = open.mzpack("D:/demo/test.mzPack");
let ionsList = MSI::getMatrixIons(rawdata, mzdiff = 0.01, q= 0.001, fast_bins = TRUE);

write.csv(ionsList, file = "D:/demo/test_ions.csv");