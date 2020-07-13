imports ["mzML.ERS", "assembly"] from "mzkit";

setwd(!script$dir);

let raw as string = "D:\biodeep\B062.mzML";
let IC as string = get_instrument(raw);

if (is.null(IC)) {
	stop("No electromagnetic radiation spectrum detector device data was found!");
} else {
	raw = raw.scans(raw);
}

print("electromagnetic radiation spectrum detector device is:");
print(IC);
print(raw);

raw
:> extract_UVsignals(instrumentId = IC)
:> as.UVtime_signals
:> write.UVsignals(file = "./UVtest.cdf")
;