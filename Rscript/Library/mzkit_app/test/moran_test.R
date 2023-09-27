require(mzkit);

imports ["mzweb","MSI"] from "mzkit";
imports "MsImaging" from "mzplot";

const rawdata = open.mzpack("D:\biodeep\biodeepdb_v3\spatial\msi_conv\test\rotate-gan-4.mzPack");
const index = MsImaging::viewer(rawdata);
const target = MSIlayer(index,  611.9421, tolerance = "da:0.05");

str(as.list(ionStat(target)));

let all_ions = MSI::ionStat(rawdata);

print(as.data.frame(all_ions));

print(all_ions$pixels);