require(mzkit);

imports "GCxGC" from "mzkit";

let rawdata = open.mzPack("E:\\D1.mzPack");
let gcxgc = GCxGC::demodulate_2D(rawdata, modtime = 4);