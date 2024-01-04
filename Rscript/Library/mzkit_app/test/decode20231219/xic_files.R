require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("G:\\tmp\\QC_pos", pattern = "*.mzPack");

print(basename(files));

for(file in files) {
    let rawdata = open.mzpack(file);
    let xic = mz.groups(ms1 = rawdata, mzdiff = "da:0.01");

    print(file);
    writeBin(xic, con = `${dirname(file)}/${basename(file)}.xic`);
}