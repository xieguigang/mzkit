require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("E:\\lipids-XCMS3-rt\\raw\\pos_mzPack", pattern = "*.xic");
let pool = xic_pool(files);
let mzbins = read.csv("E:\\lipids-XCMS3-rt\\raw\\pos_mzbins.csv", row.names = 1, check.names = FALSE);

print(mzbins);

let matrix = mz_deco(pool ,feature = 711.5664, tolerance = "da:0.01");