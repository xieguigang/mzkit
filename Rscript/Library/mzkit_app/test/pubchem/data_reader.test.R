require(mzkit);

imports ["pubchem_kit" "massbank"] from "mzkit";

const pubchem_source = "F:\compounds\PubChem_compound_text_kegg.xml";
const msgpack = "F:\compounds\PubChem_compound_text_kegg.msgpack";

pubchem_source
|> read.webquery(convert.std = TRUE)
|> as.vector()
|> write.metalib(file = msgpack)
;

let metadb = readBin(msgpack, what = "metalib");

print(metadb);