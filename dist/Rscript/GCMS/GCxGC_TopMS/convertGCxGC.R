imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

using cdf as open.netCDF("F:\Lu6-2.cdf") {
	cdf 
	|> as.mzpack(modtime = 5)
	|> write.mzPack(file = "F:\Lu6-2.mzpack")
	;	
}