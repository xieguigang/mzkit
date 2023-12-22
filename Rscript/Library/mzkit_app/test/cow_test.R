require(mzkit);

imports "mzDeco" from "mz_quantify";

let cachefiles = list.files("G:\\tmp\\pos_mzPack", pattern = "*.dat");
let samples = lapply(cachefiles, function(path) {
    read.peakFeatures(path, readBin = TRUE);
});

names(samples) = basename(cachefiles);

print(names(samples));

let peaktable = peak_alignment(samples = samples);

write.csv(peaktable, file = "G:\\tmp\\pos.csv", row.names = TRUE);