require(mzkit);

imports "MoleculeNetworking" from "mzDIA";
imports "mzDeco" from "mz_quantify";

let source_dir = "\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack";
let rawfiles = list.files(source_dir, pattern = "*.mzPack");

print(basename(rawfiles ));

let peaks = read.xcms_peaks("\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzXML\peak_tablenew.txt", 
    tsv = TRUE);

peaks <- mzkit::preprocessing_expression(peaks, 
        sampleinfo = NULL, 
        factor = 1e8, missing = 0.9
    );

let rawdata = rawfiles 
|> as.list(names = basename(rawfiles)) 
|> tqdm() 
|> lapply(function(filepath) {
    open.mzpack(filepath, verbose = FALSE);
})
;
let assigned = rawdata |> spectrum_grid() |> grid_assigned(peaks);


assigned
|> JSON::json_encode()
|> writeLines(con = "\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack\test.json")
;