require(mzkit);

imports "mzDeco" from "mz_quantify";

let rawfiles = list.files("G:\\tmp\\pos_mzPack", pattern = "*.mzPack");

print(rawfiles);

for(file in rawfiles) {
    let rawdata = open.mzpack(file);
    let ms1 = rawdata |> ms1_scans();
    let peaks = mz_deco(ms1, tolerance = "da:0.005", peak.width = [6,30], joint = FALSE);
    let debug_file = `${dirname(file)}/${basename(file)}.dat`;

    write.peaks(peaks, file = debug_file);
}