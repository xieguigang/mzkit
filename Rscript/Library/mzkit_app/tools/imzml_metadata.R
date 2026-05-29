require(mzkit);

imports "MSI" from "mzkit";

let imzml = ?"--imzml" || stop("no imzML file input was specific!");
let header = read.imzml_metadata(imzml);

print(as.data.frame(header));