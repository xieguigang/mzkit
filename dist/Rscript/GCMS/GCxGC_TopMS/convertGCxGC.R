imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

using cdf as open.netCDF("F:\CONTROL-3_1.cdf") {
	cdf 
	|> as.mzpack(modtime = 4)
	|> write.mzPack(file = "F:\CONTROL-3_1.mzpack")
	;	
}