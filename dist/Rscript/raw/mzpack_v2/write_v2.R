require(mzkit);
require(HDS);

imports "mzPack" from "mzkit";
imports "mzweb" from "mzkit";

data = open.mzpack("E:\mzkit\DATA\test\Angiotensin_AllScans.mzML");
v2file = `${@dir}/Angiotensin_AllScans.mzPack`;

mzPack::packStream(data, file = v2file);

data = HDS::openStream(v2file);

print(HDS::tree(data, showReadme = FALSE));