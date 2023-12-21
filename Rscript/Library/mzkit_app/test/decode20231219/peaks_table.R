require(mzkit);

imports "mzDeco" from "mz_quantify";

let xic = list.files("E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic", pattern = "*.XIC");
xic = lapply(xic, path -> readBin(path, what = "mz_group"), names = basename(xic));

xic 
|> mz_deco()
|> write.csv(
    file = "E:\biodeep\lipids-XCMS3-rt\raw\test_QC_dtw_xic.csv"
);