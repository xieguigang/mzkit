require(mzkit);

let source_dir = "\\192.168.1.254\backup3\项目以外内容\human_reference_metabolome\benchmark\MTBLS6039\FILES\RAW_FILES\POS\mzPack";
let rawfiles = list.files(source_dir, pattern = "*.mzPack");

print(basename(rawfiles ));

let rawdata = rawfiles 
|> as.list(names = basename(rawfiles)) 
|> tqdm() 
|> lapply(function(filepath) {
    open.mzpack(filepath, verbose = FALSE);
})
;

