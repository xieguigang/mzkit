require(mzkit);

imports "mzweb" from "mzkit";

write.cdf(file = "\\192.168.1.254\backup3\项目以外内容\2021\研发\KYYF0006-空间代谢组学\20210723\raw.netcdf") {
	open.mzpack("\\192.168.1.254\backup3\项目以外内容\2021\研发\KYYF0006-空间代谢组学\20210723\MSI.mzPack")
}
