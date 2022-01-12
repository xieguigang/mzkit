require(mzkit);

imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

cdfpath  = ?"--cdf"     || stop("no cdf file provided!");
mzfile   = ?"--output"  || `${dirname(cdfpath)}/${basename(cdfpath)}.mzPack`;
mod_time = ?"--modtime" || 5;

if (!file.exists(cdfpath)) {
	stop(`missing raw data file at location: ${cdfpath}!`);
}

using cdf as open.netCDF(cdfpath) {
	cdf 
	|> as.mzpack(modtime = mod_time)
	|> write.mzPack(file = mzfile)
	;	
}