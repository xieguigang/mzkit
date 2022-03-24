imports "math" from "mzkit";
imports "visual" from "mzplot";

require(buffer);

setwd(@dir);

#region "demo data"

# the raw data base64 string copy from the peaks node
# of a demo mzxml file
const raw as string = "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";

#endregion

let runDecode_demo as function(raw as string) {

	# the steps in R# scripting for decode the ms matrix 
	# from the raw data string 
	const data_decode as double = raw
	# decode the base64 string at first for get the raw byte stream data
	|> base64_decode      
	# the demo raw bytes data stream is compressed in zlib format
	# do zlib decompression of the stream data
	|> gzip.decompress
	# then we run bit convert of the byte stream data as floats number array
	|> float(networkOrder = TRUE, sizeOf = 64)
	;

	print("demo decode of the raw MS data:");
	str(data_decode);

	# mzXML is mz-into tuple data
	# odds is m/z
	# and even is intensity value
	const mz as double   = data_decode[1:length(data_decode) step 2];
	const into as double = data_decode[2:length(data_decode) step 2];
	
	# finally we have the ms matrix
	# decoded from the mzxml file
	ms_matrix = data.frame("m/z" = mz, intensity = into);
	ms_matrix = ms_matrix[ order(ms_matrix[, "intensity"], decreasing = TRUE), ];	

	cat("\n\n");

	print("previews of your decoded ms matrix output from the raw data:");
	print(head(ms_matrix));
	
	ms_matrix;
}

bitmap(file = `${dirname(@script)}/raw_decodeDemo.png`) {

	# run demo data decode and then plot generated 
	# matrix object to a bitmap file
	raw
	|> runDecode_demo()
	|> centroid
	|> plot
	;
}

