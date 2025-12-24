require(mzkit);

imports "mzweb" from "mzkit";

# 转换文件格式为mzkit支持的文件格式
write.mzPack(open.mzpack("D:/demo/test.imzML"), file = "D:/demo/test.mzPack");
