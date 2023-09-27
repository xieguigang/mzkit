require(mzkit);

imports ["mzweb","MSI"] from "mzkit";
imports "MsImaging" from "mzplot";

const rawdata = MsImaging::viewer(open.mzpack("D:\biodeep\biodeepdb_v3\spatial\msi_conv\test\rotate-gan-4.mzPack"));
const target = MSIlayer(rawdata,  611.9421, tolerance = "da:0.05");

str(as.list(ionStat(target)));