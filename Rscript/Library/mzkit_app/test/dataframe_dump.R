require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "massbank" from "mzkit";

let mapping = readText("G:\tmp\.pubchem_cache\mapping.json") |> JSON::json_decode();
let files = list.files("G:\tmp\.pubchem_cache\pugViews", pattern = "*.xml", recursive = TRUE);
let metadata = sapply(files, path -> read.pugView(path));

mapping = lapply(mapping, cid -> `PubChem:${cid}`);
metadata = as.data.frame(metadata);

let cols = colnames(metadata);

metadata = as.list(metadata, byrow = TRUE);

str(metadata);

let join = [];

for(let q in names(mapping)) {
    for(let id in mapping[[q]]) {
        let xi =as.list( metadata[[id]]);
        if (!is.null(xi)) {
            xi$query = q;
            join = append(join, xi);
        }
    }
}

str(join);

metadata = data.frame(
    query = join@query
);

for(name in cols) {
    metadata[, name] = sapply( join, i -> i[[name]]);
}

write.csv(metadata, file = "F:/metabolite_exports.csv");
