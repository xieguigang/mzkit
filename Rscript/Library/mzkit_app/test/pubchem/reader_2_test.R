require(HDS);
require(JSON);

const pack = HDS::openStream("D:\\extension.dat", readonly = TRUE);
# /03/pathways_6029.json
const json_str = HDS::getText(pack, fileName = "/03/pathways_6029.json");
const pathwayList = JSON::json_decode(json_str);

str(pathwayList);

const json_files = HDS::files(pack, excludes_dir = TRUE, size_desc = TRUE);
const json_str2 = HDS::getText(pack, fileName = json_files[1]);

writeLines(json_str2, con = file.path(@dir,  "invalid_json.json"));

str(JSON::json_decode(json_str2, strict.vector.syntax = FALSE));