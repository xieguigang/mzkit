require(mzkit);

imports "pubchem_kit" from "mzkit";

xml = read.pugView("E:\mzkit\src\mzkit\rstudio\data\compound_CID_2244.xml");
metadata = metadata.pugView(xml);

str(as.list(metadata));