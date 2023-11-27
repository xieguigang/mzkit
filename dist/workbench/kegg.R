# helper script for imports pubchem kegg compound list into GCModeller package

require(GCModeller);
require(mzkit);

imports "pubchem_kit" from "mzkit";

const hits = read.webquery("E:\\PubChem_compound_text_kegg.xml");

for(metabo in hits) {
    metabo = as.list(metabo);
    str(metabo);
}