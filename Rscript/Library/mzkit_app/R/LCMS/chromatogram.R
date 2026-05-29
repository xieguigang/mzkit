imports "visual" from "mzplot";
imports "chromatogram" from "mzkit";

const make_chromatogram_exports = function(rawdir, sampleinfo, group_name, 
                                           export_dir = "./", 
                                           overlaps_size = [2900,1600],
                                           overlaps_layout = "padding:5% 5% 10% 12%;", 
                                           file_color = "blue") {
    
    if (!is.data.frame(sampleinfo)) {
        sampleinfo = read.csv(sampleinfo, row.names = NULL, check.names = FALSE);
    }

    sampleinfo <- sampleinfo[sampleinfo$sample_info == group_name, ];

    if (nrow(sampleinfo) == 0) {
        return(NULL);
    } else {
        sampleinfo = sampleinfo |> groupBy("ID"); 
    }

    let overlaps_data = new("overlaps") ;

    for(let file in list.files(rawdir, pattern = c("*.mzXML","*.mzML"))) {
        if (basename(file) in sampleinfo) {
            let rawdata = open.mzpack(file);
            let tic_data = TIC(rawdata);
            let bpc_data = BPC(rawdata);
            let label = sampleinfo[[basename(file)]]$sample_name;
            let filedata = toChromatogram(tic = tic_data, bpc = bpc_data, 
                name = label);

            bitmap(file = file.path(export_dir, "files", `TIC_${label}.png`)) {
                plot(filedata, name = `TIC - ${label}`, color = file_color);
            }
            pdf(file = file.path(export_dir,"files", `TIC_${label}.pdf`)) {
                plot(filedata, name = `TIC - ${label}`, color = file_color);
            }

            bitmap(file = file.path(export_dir,"files", `BPC_${label}.png`)) {
                plot(filedata, bpc = TRUE, name = `BPC - ${label}`, color = file_color);
            }
            pdf(file = file.path(export_dir,"files", `BPC_${label}.pdf`)) {
                plot(filedata, bpc = TRUE, name = `BPC - ${label}`, color = file_color);
            }

            overlaps_data <- overlaps_data + filedata;
        }       
    }

    bitmap(file = file.path(export_dir, `TIC_${basename(dirname(rawdir))}.png`)) {
        plot(overlaps_data, size = overlaps_size, fill = FALSE, padding = overlaps_layout, colors = "paper");
    }
    pdf(file = file.path(export_dir, `TIC_${basename(dirname(rawdir))}.pdf`)) {
        plot(overlaps_data, size = overlaps_size, fill = FALSE, padding = overlaps_layout, colors = "paper");
    }

    bitmap(file = file.path(export_dir, `BPC_${basename(dirname(rawdir))}.png`)) {
        plot(overlaps_data, bpc = TRUE, size = overlaps_size, fill = FALSE, padding = overlaps_layout, colors = "paper");
    }
    pdf(file = file.path(export_dir, `BPC_${basename(dirname(rawdir))}.pdf`)) {
        plot(overlaps_data, bpc = TRUE, size = overlaps_size, fill = FALSE, padding = overlaps_layout, colors = "paper");
    }
}