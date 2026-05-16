require(mzkit);

imports "visual" from "mzplot";
imports "chromatogram" from "mzkit";

let rawdir = "\\192.168.3.15\sda\2026\wzc\1\pos";
let export_dir = "Z:/";
let sampleinfo = read.csv("\\192.168.3.15\sda\2026\wzc\1\sampleinfo.csv", row.names = NULL, check.names = FALSE);
let group_name = "QC";

sampleinfo = sampleinfo[sampleinfo$sample_info == group_name, ];

if (nrow(sampleinfo) > 0) {
    sampleinfo = sampleinfo |> groupBy("ID"); 
    
    for(let file in list.files(rawdir, pattern = c("*.mzXML","*.mzML"))) {
        if (basename(file) in sampleinfo) {
            let rawdata = open.mzpack(file);
            let tic_data = TIC(rawdata);
            let bpc_data = BPC(rawdata);
            let label = sampleinfo[[basename(file)]]$sample_name;
            let filedata = toChromatogram(tic = tic_data, bpc = bpc_data);

            bitmap(file = file.path(export_dir, `TIC_${label}.png`)) {
                plot(filedata);
            }
            pdf(file = file.path(export_dir, `TIC_${label}.pdf`)) {
                plot(filedata);
            }

            bitmap(file = file.path(export_dir, `BPC_${label}.png`)) {
                plot(filedata, bpc = TRUE);
            }
            pdf(file = file.path(export_dir, `BPC_${label}.pdf`)) {
                plot(filedata, bpc = TRUE);
            }
        }       
    }
}

