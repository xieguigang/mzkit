require(mzkit);

imports "annotation" from "mzkit";

let peaktable = "Z:\项目以外内容\2023\血清血浆物质列表\lipidomics_test\blood\peak_tablenew.txt";
let packfile = open.annotation_workspace("Z:\项目以外内容\2023\血清血浆物质列表\lipidomics_test\blood\HCS_new_20240807-1\MZKit.hdms", 
                    io = "Write");
    let peakdata = read.xcms_peaks(peaktable, 
                    tsv = file.ext(peaktable) != "csv", 
                    general_method = FALSE);
    
    write.xcms_peaks(peakdata, file = packfile);