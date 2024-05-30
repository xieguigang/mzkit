require(mzkit);

imports "mzDeco" from "mz_quantify";

let peaks = read.xcms_peaks(
    file = "\\192.168.1.254\backup3\项目以外内容\全靶测试\100mm_方法缩短\20240523_8+8min\035\neg\test_peaksdata_0-8min_MIX_CA_1.csv",
            tsv = FALSE);