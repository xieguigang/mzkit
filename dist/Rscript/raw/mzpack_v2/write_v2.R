require(mzkit);
require(HDS);
require(JSON);

imports "mzPack" from "mzkit";
imports "mzweb" from "mzkit";

data = open.mzpack("/mzkit\DATA\test\Angiotensin_AllScans.mzML");
v2file = `${@dir}/Angiotensin_AllScans.mzPack`;

mzPack::packStream(data, file = v2file);

data = HDS::openStream(v2file);

print(HDS::tree(data, showReadme = FALSE));

HDS::getText(data, "/.etc/metadata.json")
|> json_decode()
|> str()
;


HDS::getText(data, "/.etc/ms_scans.json")
|> json_decode()
|> str()
;

close(data);

data = open.mzpack(v2file);

str(data);