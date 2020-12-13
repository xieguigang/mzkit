imports "massbank" from "mzkit";

read.SDF("D:\LMSD.sdf\structures.sdf")
:> as.lipidmaps
:> as.vector
:> as.data.frame
:> write.csv(file = "D:\LMSD.sdf\lipidmap.csv")
;