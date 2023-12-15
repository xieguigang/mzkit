require(mzkit);

imports "GCxGC" from "mzkit";

let rawdata = open.mzpack("E:\\D1.mzPack");
let gcxgc = GCxGC::demodulate_2D(rawdata, modtime = 4);

write.mzPack(gcxgc, file = `${@dir}/D1_GCxGC.mzPack`);