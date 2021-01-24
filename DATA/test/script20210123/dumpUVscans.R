imports ["mzML.ERS", "assembly"] from "mzkit";

setwd(dirname(@script));

# config of the source mzML data file input and 
# the UV scan cdf data file output.
const raw as string    = "E:/D065.mzML";
const output as string = "demo_UVscans.cdf";

# get instrument configuration of the UV scan data.
let IC as string = get_instrument(raw);

if (is.null(IC)) {
	stop(`No electromagnetic radiation spectrum detector device data was found in '${raw}'!`);
} else {
	let rawData = raw.scans(raw);
	print("electromagnetic radiation spectrum detector device is:");
	print(IC);
	print(rawData);

	# extract the UV scan data from raw data file and then
	# export to a netCDF file.
	rawData
	:> extract_UVsignals(instrumentId = IC)
	# :> as.UVtime_signals
	:> write.UVsignals(file = output)
	;
}
