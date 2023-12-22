require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("E:\\biodeep\\lipids-XCMS3-rt\\raw\\neg_mzPack", pattern = "*.mzPack");

print(basename(files));

for(file in files) {
    let rawdata = open.mzpack(file);
    let xic = mz.groups(ms1 = rawdata, mzdiff = "da:0.01");

    print(file);
    writeBin(xic, con = `${dirname(file)}/${basename(file)}.xic`);
}