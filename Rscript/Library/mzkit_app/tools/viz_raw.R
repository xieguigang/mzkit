require(mzkit);

let rawdir = "\\192.168.3.15\sda\2026\wzc\1\pos";
let export_dir = "Z:/";

for(let file in list.files(rawdir, pattern = c("*.mzXML","*.mzML"))) {
    let rawdata = open.mzpack(file);

    
}