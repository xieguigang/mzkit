require(mzkit);

let rawdata = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\rawdata\QC1.mzML";
let cachefile = "\\192.168.1.254\backup3\项目以外内容\2025\三级MSn测试\test1\tmp\.cache\raw\d\QC1.mzPack";
let loaddata = open.mzpack(rawdata);
       
loaddata |> write.mzPack(file = cachefile, version = 1);

let data = open.mzpack(cachefile);

