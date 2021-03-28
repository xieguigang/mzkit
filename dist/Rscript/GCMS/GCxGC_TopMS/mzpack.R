imports "mzweb" from "mzkit";

require(netCDF.utils);

using cdf as open.netCDF("P:\MTBLS212\CZ1_5m.cdf") {
	
	cdf 
	:> as.mzpack
	:> write.mzPack(file = "P:\MTBLS212\CZ1_5m.mzpack")
	;

}