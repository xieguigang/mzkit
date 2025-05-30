require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("G:\\tmp\\QC_pos", pattern = "*.xic");

print(files);

let xic_data = lapply(files, path -> readBin(path, what = "mz_group"));
let mz = lapply(xic_data, function(pack, i) {
    data.frame(
        mz = [pack]::mz,
        TIC = [pack]::TIC,
        maxinto = [pack]::MaxInto,
        row.names = `#${i}-${1:length(pack)}`
    );
});

let mzraw = NULL;

for(m in mz) {
    mzraw <- rbind(m, mzraw);
}

print(mzraw);

# make centroid 
mzraw = libraryMatrix(data.frame(mz = mzraw$mz, into = mzraw$TIC));
mzraw = centroid(mzraw, tolerance = "da:0.01", intoCutoff = 0);
mzraw = as.data.frame(mzraw);

print(mzraw);

write.csv(mzraw, "G:\\tmp\\QC_pos\\mzbins.csv", row.names = FALSE);