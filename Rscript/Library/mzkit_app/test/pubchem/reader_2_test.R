require(HDS);
require(JSON);

const pack = HDS::openStream("D:\\extension.dat", readonly = TRUE);
# /03/pathways_6029.json
const json_str = HDS::getText(pack, fileName = "/03/pathways_6029.json");
const pathwayList = JSON::json_decode(json_str);

str(pathwayList);
