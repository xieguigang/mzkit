imports "massbank" from "mzkit";

require(dataframe);

read.dataframe("D:\LMSD.sdf\lipidmap.csv", mode = "character")
:> lipid.nameMaps
:> write.csv("D:\LMSD.sdf\lipidmapsName.csv")
;