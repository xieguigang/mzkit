require(mzkit);

imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

cdfpath = ?"--cdf"    || stop("no cdf file provided!");
mzfile  = ?"--output" || `${dirname(cdfpath)}/${basename(cdfpath)}.mzPack`;

if (!file.exists(cdfpath)) {
	stop(`missing raw data file at location: ${cdfpath}!`);
}

using cdf as open.netCDF(cdfpath) {
	cdf 
	|> as.mzpack(modtime = 5)
	|> write.mzPack(file = mzfile)
	;	
}