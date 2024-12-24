require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "massbank" from "mzkit";

let mapping = readText("G:\tmp\.pubchem_cache\mapping.json") |> JSON::json_decode();
let files = list.files("G:\tmp\.pubchem_cache\pugViews", pattern = "*.xml", recursive = TRUE);
let metadata = sapply(files, path -> read.pugView(path));

metadata = as.data.frame(metadata);

for(let meta in metadata) {
    str(meta);
}