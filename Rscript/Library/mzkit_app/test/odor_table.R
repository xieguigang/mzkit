require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "massbank" from "mzkit";

let mapping = readText("G:\tmp\.pubchem_cache\mapping.json") |> JSON::json_decode();
let files = list.files("G:\tmp\.pubchem_cache\pugViews", pattern = "*.xml", recursive = TRUE);
let metadata = sapply(files, path -> read.pugView(path));
let data = NULL;

for(let meta in metadata) {
    meta <- metadata.pugView(meta);

    let odors_data = odors(meta);
    odors_data[,"cid"] = [meta]::ID;
    # print(odors_data);

    data = rbind(data, odors_data);
}

print(data);

write.csv(data, file = "Z:/aaa.csv", row.names = FALSE);