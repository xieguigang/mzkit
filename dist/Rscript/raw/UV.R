imports ["mzML.ERS", "assembly"] from "mzkit";

setwd(!script$dir);

let rawFolder as string = ?"--mzML";
let runFile as function(raw) {
	let output as string = `${dirname(raw)}/${basename(raw)}.cdf`;
	let IC as string     = get_instrument(raw);

	if (is.null(IC)) {
		warning(`No electromagnetic radiation spectrum detector device data was found in '${raw}'!`);
	} else {
		raw = raw.scans(raw);
		print("electromagnetic radiation spectrum detector device is:");
		print(IC);
		print(raw);

		raw
		:> extract_UVsignals(instrumentId = IC)
		# :> as.UVtime_signals
		:> write.UVsignals(file = output)
		;
	}
}

for(raw in list.files(rawFolder, pattern = "*.mzML")) {
	runFile(raw);
}

