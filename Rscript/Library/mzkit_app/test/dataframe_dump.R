require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "massbank" from "mzkit";

let files = list.files("F:\.pubchem_cache\pugViews", pattern = "*.xml", recursive = TRUE);
let metadata = sapply(files, path -> read.pugView(path));

write.csv(as.data.frame(metadata), file = "F:/metabolite_exports.csv");
