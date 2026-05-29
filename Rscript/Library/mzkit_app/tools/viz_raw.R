require(mzkit);

imports "visual" from "mzplot";
imports "chromatogram" from "mzkit";

let rawdir = "\\192.168.3.15\sda\2026\wzc\1\pos";
let export_dir = "Z:/";
let sampleinfo = read.csv("\\192.168.3.15\sda\2026\wzc\1\sampleinfo.csv", row.names = NULL, check.names = FALSE);
let group_name = "QC";
let overlaps_size = [3000, 1600];
let overlaps_layout = "padding:5% 5% 10% 15%;";

sampleinfo = sampleinfo[sampleinfo$sample_info == group_name, ];

if (nrow(sampleinfo) > 0) {
    let overlaps_data = new("overlaps") ;

    sampleinfo = sampleinfo |> groupBy("ID"); 
    
    for(let file in list.files(rawdir, pattern = c("*.mzXML","*.mzML"))) {
        if (basename(file) in sampleinfo) {
            let rawdata = open.mzpack(file);
            let tic_data = TIC(rawdata);
            let bpc_data = BPC(rawdata);
            let label = sampleinfo[[basename(file)]]$sample_name;
            let filedata = toChromatogram(tic = tic_data, bpc = bpc_data);

            bitmap(file = file.path(export_dir, `TIC_${label}.png`)) {
                plot(filedata, name = `TIC - ${label}`, color = "darkblue");
            }
            pdf(file = file.path(export_dir, `TIC_${label}.pdf`)) {
                plot(filedata, name = `TIC - ${label}`, color = "darkblue");
            }

            bitmap(file = file.path(export_dir, `BPC_${label}.png`)) {
                plot(filedata, bpc = TRUE, name = `BPC - ${label}`, color = "darkblue");
            }
            pdf(file = file.path(export_dir, `BPC_${label}.pdf`)) {
                plot(filedata, bpc = TRUE, name = `BPC - ${label}`, color = "darkblue");
            }

            overlaps_data = overlaps_data + filedata;
        }       
    }

    bitmap(file = file.path(export_dir, `TIC_${basename(dirname(rawdir))}.png`)) {
        plot(overlaps_data, size = overlaps_size, fill = FALSE, padding = overlaps_layout);
    }
    pdf(file = file.path(export_dir, `TIC_${basename(dirname(rawdir))}.pdf`)) {
        plot(overlaps_data, size = overlaps_size, fill = FALSE, padding = overlaps_layout);
    }

    bitmap(file = file.path(export_dir, `BPC_${basename(dirname(rawdir))}.png`)) {
        plot(overlaps_data, bpc = TRUE, size = overlaps_size, fill = FALSE, padding = overlaps_layout);
    }
    pdf(file = file.path(export_dir, `BPC_${basename(dirname(rawdir))}.pdf`)) {
        plot(overlaps_data, bpc = TRUE, size = overlaps_size, fill = FALSE, padding = overlaps_layout);
    }
}

