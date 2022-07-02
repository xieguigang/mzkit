require(mzkit);


imports "mzPack" from "mzkit";
imports "mzweb" from "mzkit";

data = open.mzpack("E:\mzkit\DATA\test\Angiotensin_AllScans.mzML");

mzPack::packStream(data, file = `${@dir}/Angiotensin_AllScans.mzPack`);
