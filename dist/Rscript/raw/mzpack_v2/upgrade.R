require(mzkit);

imports "mzweb" from "mzkit";

data = open.mzpack("F:\C00041_L-Alanine.mzPack");

write.mzPack(data, file = "D:\mzkit\src\mzkit\rstudio\data\C00041_L-Alanine.mzPack");