require(mzkit);

imports "mzDeco" from "mz_quantify";
imports "visual" from "mzplot";

let files = list.files("E:\biodeep\lipids-XCMS3-rt\raw\raw_test_QC", pattern = "*.xic");
let pool = xic_pool(files);
let mzbins = read.csv("E:\biodeep\lipids-XCMS3-rt\raw\pos_mzbins.csv", row.names = 1, check.names = FALSE);

print(mzbins);

# const demo1 = 150.14962768554688;
# const demo2 = 711.5664;

# let raw_xic = pull_xic(pool , mz = demo1, dtw = FALSE);
# let dtw_xic = pull_xic(pool, mz = demo1, dtw = TRUE);

let peaktable = [];

for(mz in mzbins$mz) {
    str(mz);

    let dtw_xic = pull_xic(pool, mz = mz, dtw = TRUE);
    let key = md5(as.character(mz));

    bitmap(file = `E:/biodeep/lipids-XCMS3-rt/raw/test_QC_dtw_xic_plot/${substr(key, 5,6)}/${mz}.png`) {
        raw_snapshot3D(dtw_xic);
    }

    peaktable = append(peaktable, pool 
    |> mz_deco(feature = mz, joint = TRUE, peak.width = [3,60]));

    NULL;
}

write.csv(peaktable, file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.csv");



# for(name in names(dtw_xic)) {
#     writeBin(dtw_xic[[name]], con = `E:/biodeep/lipids-XCMS3-rt/raw/test_QC_dtw_xic/${name}.XIC`);
# }

# bitmap(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_raw_xic.png") {
#     raw_snapshot3D(raw_xic);
# }

# bitmap(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.png") {
#     raw_snapshot3D(dtw_xic);
# }

# pool 
# |> mz_deco(feature = 711.5664, joint = TRUE, peak.width = [3,60])
# |> write.csv(file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.csv")
# ;