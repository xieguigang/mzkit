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
let grid = rawdata |> spectrum_grid();
let assigned = grid |> grid_assigned(peaks);
let unassigned = unpack_unmapped(grid);
let stats = {
    total_clusters: attr(unassigned,"total_clusters"),
    total_spectrum: attr(unassigned,"total_spectrum"),
    unmapped_clusters: attr(unassigned,"unmapped_clusters"),
    unmapped_spectrum: attr(unassigned,"unmapped_spectrum")
}

str(stats );

print("cluster unmapping ratio: ");
print(`${100*as.integer(attr(unassigned,"unmapped_clusters"))/as.integer(attr(unassigned,"total_clusters"))}% (${attr(unassigned,"unmapped_clusters")}/${attr(unassigned,"total_clusters")})`);
print("spectrum unmapping ratio:");
print(`${100*as.integer(attr(unassigned,"unmapped_spectrum"))/as.integer(attr(unassigned,"total_spectrum"))}% (${attr(unassigned,"unmapped_spectrum")}/${attr(unassigned,"total_spectrum")})`);

write.csv(as.data.frame(assigned), file = "\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack\test.csv");

assigned
|> JSON::json_encode()
|> writeLines(con = "\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack\test.json")
;

assigned = unpack_assign(assigned);

for(let name in names(assigned)) {
    write.cache(assigned[[name]], file = `\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack\cache/${name}.cache`);
}

for(let name in names(unassigned)) {
    write.cache(unassigned[[name]], file = `\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack\unmapped/${name}.cache`);
}