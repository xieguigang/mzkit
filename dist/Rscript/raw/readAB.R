require(mzkit);

imports "mzweb" from "mzkit";

let raw = open.mzpack("F:\P220503919.mzXML");

write.mzPack(raw, file = "F:\P220503919_test.mzPack");