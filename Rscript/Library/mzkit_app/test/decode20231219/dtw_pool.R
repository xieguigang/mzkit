require(mzkit);

imports "mzDeco" from "mz_quantify";
imports "visual" from "mzplot";

let files = list.files("E:\biodeep\lipids-XCMS3-rt\raw\raw_test_QC", pattern = "*.xic");
let pool = xic_pool(files);
# let mzbins = read.csv("E:\\lipids-XCMS3-rt\\raw\\pos_mzbins.csv", row.names = 1, check.names = FALSE);

# print(mzbins);

let raw_xic = pull_xic(pool , mz = 711.5664, dtw = FALSE);
let dtw_xic = pull_xic(pool, mz = 711.5664, dtw = TRUE);

for(name in names(dtw_xic)) {
    writeBin(dtw_xic[[name]], con = `E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic\${name}.XIC`);
}

bitmap(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_raw_xic.png") {
    raw_snapshot3D(raw_xic);
}

bitmap(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.png") {
    raw_snapshot3D(dtw_xic);
}

pool 
|> mz_deco(feature = 711.5664)
|> write.csv(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.csv")
;