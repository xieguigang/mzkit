require(mzkit);

imports "mzDeco" from "mz_quantify";

let cachefiles = list.files("G:\\tmp\\pos_mzPack", pattern = "*.dat");
let samples = lapply(cachefiles, function(path) {
    read.peakFeatures(path, readBin = TRUE);
});

names(samples) = basename(samples);

str(samples);