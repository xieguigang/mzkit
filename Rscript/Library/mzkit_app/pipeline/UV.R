imports ["mzML.ERS", "assembly"] from "mzkit";

[@info "the folder path that contains multiple mzML raw data file."]
[@type "directory"]
const rawFolder as string = ?"--mzML" || stop("no data source folder path!");
const runFile as function(raw) {	
	const IC as string = get_instrument(raw);

	if (is.null(IC)) {
		warning(`No electromagnetic radiation spectrum detector device data was found in '${raw}'!`);
	} else {
		raw = raw.scans(raw);
		print("electromagnetic radiation spectrum detector device is:");
		print(IC);
		print(raw);

		raw
		|> extract_UVsignals(instrumentId = IC)
		|> write.UVsignals(file = `${dirname(raw)}/${basename(raw)}.cdf`)
		;
	}
}

for(raw in list.files(rawFolder, pattern = "*.mzML")) {
	runFile(raw);
}

