require(mzkit);
require(xcms);

mzXML    <- "{$file.mzXML}";
save.mgf <- "{$output.mgf}";

# get mzXML raw data
raw <- 