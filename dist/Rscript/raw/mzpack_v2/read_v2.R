require(mzkit);
require(HDS);
require(JSON);

imports "mzPack" from "mzkit";
imports "mzweb" from "mzkit";

const v2raw = "V:\project\CRO\Test_Brain_chca(10MG0.1%FA)_Tri_L15_15%_10mmmin20220628\20220705-1\analysis\06.MZKit数据查看\CleanSample.mzPack";

data = HDS::openStream(v2raw);

# print(HDS::tree(data, showReadme = FALSE));
close(data);

data = open.mzpack(v2raw);