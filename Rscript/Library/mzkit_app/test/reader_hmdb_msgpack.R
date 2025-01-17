require(mzkit);

imports "hmdb_kit" from "mzkit";

setwd(@dir);

let s = readBin(file.path(@dir, "/../data/hmdb.msgpack"), what = "hmdb");

str(s[1]);