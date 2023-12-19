require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("E:\\lipids-XCMS3-rt\\raw\\pos_mzPack", pattern = "*.xic");
let pool = xic_pool(files);

