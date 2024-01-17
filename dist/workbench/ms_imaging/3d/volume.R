require(mzkit);

imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";

options(memory.load = "max");

let power = MsImaging::parseFilters(["cut(0.1, FALSE)" "TrIQ_clip(0.85,100)" "denoise(0.01)"]);
let rawfiles = list.files("\\192.168.1.254\backup3\项目以外内容\2024\动物器官3D重建测试\raw_mzPack\single_ions\413.0958", pattern = "*.dat");
let z = as.numeric(basename(rawfiles));

rawfiles = rawfiles[order(z)];

let load = rawfiles 
|> lapply(path -> readBin(path, what = "msi_layer"))
|> intensityFilter(power)
;

MSI::z_volume(
    layers = load, 
    dump = "E:\mzkit\src\mzkit\webview\assets\demo_3dmaldi"
);