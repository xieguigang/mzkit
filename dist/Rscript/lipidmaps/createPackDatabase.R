require(mzkit);

imports "massbank" from "mzkit";

"D:\Database\20220306\repo\structures.sdf"
|> read.SDF(parseStruct = FALSE)
|> as.lipidmaps()
|> write.lipidmaps(file = "D:\mzkit\Rscript\Library\mzkit_app\data\LIPIDMAPS.msgpack")
;