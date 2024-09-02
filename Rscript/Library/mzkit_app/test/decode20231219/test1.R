require(mzkit);

let rawdata = "G:\lxy-CID30.mzPack";
let peaks = MS1deconv(rawdata, massdiff = 0.01,
peak_width = [3,12],
 q  = 0.65,
 sn_threshold = 1,
 nticks  = 6,
 joint = TRUE);

write.csv(peaks, file = "G:\lxy-CID30.csv");