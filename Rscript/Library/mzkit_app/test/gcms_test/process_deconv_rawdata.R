require(mzkit);

deconv_gcms(rawdata = "G:\demo\gcms-test1\RAW_FILES", 
export = "G:\demo\gcms-test1\DERIVED_FILES", peak.width = [3, 60], 
n_threads = 1);