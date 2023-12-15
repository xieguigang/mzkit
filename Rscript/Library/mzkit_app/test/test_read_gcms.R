require(mzkit);

imports "GCxGC" from "mzkit";

let rawdata = open.mzpack(file = "E:\\D1.cdf");
# let GCxGC = GCxGC::extract_2D_peaks(rawdata);
write.mzPack(rawdata, file = `${@dir}/D1.mzPack`);