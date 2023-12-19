require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("E:\\lipids-XCMS3-rt\\raw\\pos_mzPack", pattern = "*.xic");

let xic_data = lapply(files[1:3], path -> readBin(path, what = "mz_group"));
let mz = lapply(xic_data, function(pack) {
    data.frame(
        mz = [pack]::mz,
        TIC = [pack]::TIC,
        maxinto = [pack]::MaxInto
    );
});

print(mz);