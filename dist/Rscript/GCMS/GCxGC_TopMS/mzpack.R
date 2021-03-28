imports "mzweb" from "mzkit";

require(netCDF.utils);

using cdf as open.netCDF("P:\MTBLS447\WT_ Regenerated Shoot 1-1.cdf") {
	
	cdf 
	:> as.mzpack
	:> write.mzPack(file = "P:\MTBLS447\WT_ Regenerated Shoot 1-1.mzpack")
	;

}