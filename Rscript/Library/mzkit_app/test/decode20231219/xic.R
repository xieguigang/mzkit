require(mzkit);

imports "mzDeco" from "mz_quantify";

let rawdata = open.mzpack("E:\\lipids-XCMS3-rt\\raw\\pos_mzPack\\HB1.mzPack");
let xic = mz.groups(ms1 = rawdata, mzdiff = "da:0.01");

writeBin(xic, con = `${@dir}/demo.dat`);