require(mzkit);

imports "mzDeco" from "mz_quantify";

let files = list.files("E:\biodeep\lipids-XCMS3-rt\raw\raw_test_QC", pattern = "*.xic");
let pool = xic_pool(files);
# let mzbins = read.csv("E:\\lipids-XCMS3-rt\\raw\\pos_mzbins.csv", row.names = 1, check.names = FALSE);

# print(mzbins);

let raw_xic = pull_xic(pool , mz = 711.5664, dtw = FALSE);
let dtw_xic = pull_xic(pool, mz = 711.5664, dtw = TRUE);

