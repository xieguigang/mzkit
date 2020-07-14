imports ["mzML.ERS", "assembly"] from "mzkit";

setwd(!script$dir);

let runMgfDumping as function(raw, output) {

	let mzxml as string  = raw;
	let IC as string     = get_instrument(raw);
	let UVfile as string = `${output}/${basename(raw)}.cdf`;
	let mgf as string    = `${output}/${basename(raw)}.mgf`; 

	if (is.null(IC)) {
		warning("No electromagnetic radiation spectrum detector device data was found!");
	} else {
		raw = raw.scans(raw);
		
		print("electromagnetic radiation spectrum detector device is:");
		print(IC);
		print(raw);

		raw
		:> extract_UVsignals(instrumentId = IC)
		:> as.UVtime_signals
		:> write.UVsignals(file = UVfile)
		;
	}

	print(`Mgf ions data will be written to: ${mgf}`);

	mzxml 
	:> mzxml.mgf(relativeInto = FALSE, onlyMs2 = FALSE) 
	:> write.mgf(file = mgf)
	;
}

let runFolder as function(folder) {
	let raw.mzMLfiles = list.files(folder);
	let raw_output = `${dirname(folder)}/raw`;
	
	print("apply data conversion dumping pipeline for raw files:");
	print(basename(raw.mzMLfiles));
	print("raw data will be save at location:");
	print(raw_output);
	
	lapply(raw.mzMLfiles, file -> runMgfDumping(file, raw_output));
}

runFolder("F:\pos_mzML");
runFolder("F:\neg_mzML");
