imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

using cdf as open.netCDF("D:\web\Lu6-1.cdf") {
	cdf 
	|> as.mzpack(modtime = 5)
	|> write.mzPack(file = "D:\web\Lu6-1.Mzpack")
	;	
}