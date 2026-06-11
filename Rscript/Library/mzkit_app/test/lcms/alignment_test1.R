require(mzkit);

let peaks = align_peaktable(peaks_dir = "N:\2026\wzc\C2026051977612\lcms_out-20260612-1\tmp\workflow_tmp\rawdata\pos\peaksdata\peaks", mzdiff = 0.01);

print(as.data.frame(peaks));