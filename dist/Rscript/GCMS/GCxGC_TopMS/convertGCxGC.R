require(mzkit);

imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";
imports "GCxGC" from "mzkit";

cdfpath  = ?"--cdf"     || stop("no cdf file provided!");
mzfile   = ?"--output"  || `${dirname(cdfpath)}/${basename(cdfpath)}.mzPack`;
mod_time = ?"--modtime" || 5;

image_TIC = `${dirname(mzfile)}/${basename(mzfile)}.2D.png`;

if (!file.exists(cdfpath)) {
	stop(`missing raw data file at location: ${cdfpath}!`);
}

using cdf as open.netCDF(cdfpath) {
	cdf 
	|> as.mzpack(modtime = mod_time)
	|> write.mzPack(file = mzfile)
	;	
}

raw   = open.mzpack( mzfile );
gcxgc = GCxGC::extract_2D_peaks(raw);
plt = plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;", TrIQ = 1);

bitmap(plt, file = image_TIC);