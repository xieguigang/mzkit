require(mzkit);

let rawdata = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\rawdata\QC1.mzML";
let cachefile = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\test1\tmp\.cache\raw\d\QC1.mzPack";
let mgf_ascii = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\test1\samples\QC1\ms2_ascii_cache.mgf";
let cache = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\test1\samples\QC1\ionPeaks.dat";

# let loaddata = open.mzpack(rawdata);
       
# loaddata |> write.mzPack(file = cachefile, version = 1);

let data = open.mzpack(cachefile);
let ions = ms2_peaks(data,loadProductTree=TRUE);

assembly::write.mgf(ions, file = mgf_ascii);
mzweb::write.cache(ions, file = cache, 
    tag_filesource = FALSE);


let ions2 = mzweb::read.cache(cache);