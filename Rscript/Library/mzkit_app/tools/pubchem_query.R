require(mzkit);

imports "pubchem_kit" from "mzkit";

[@info "A query term input file, should be a collection of the query names. 
a simple text file in format of one metabolite name per line."]
let input = ?"--query" || stop("A given set of the input query file must be provided!");
let cache = ?"--cache" || file.path(dirname(input),".pubchem_cache");
let names = readLines(input);

names <- as.list(names, names = names);

let mapping = lapply(names, function(term) {
    let cid = pubchem_kit::CID(term, cache = cache);

    for(let id in cid) {
        pugView(id, cache);
    }

    cid;
});

str(mapping);

mapping 
|> JSON::json_encode()
|> writeLines(con = file.path(cache, "mapping.json"))
;
