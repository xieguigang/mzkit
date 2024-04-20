require(mzkit);

let file = "G:\demo\gcms-test1\RAW_FILES\free_MER_12_D3.mzPack";
let peaks = __deconv_gcms_single(file, peak.width = [3, 90]);

setwd(@dir);

writeBin(peaks, con = "./demodata.dat");


print(readBin("./demodata.dat", what = "gcms_peak"));