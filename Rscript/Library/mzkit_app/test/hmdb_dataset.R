require(mzkit);

imports ["hmdb_kit" "massbank"] from "mzkit";

const hmdb_source = "F:\compounds\hmdb_metabolites.xml";
const metadb_pack = "F:\compounds\hmdb_metabolites.msgpack";

hmdb_source
|> read.hmdb(convert.std = TRUE)
|> write.metalib(file = metadb_pack)
;
