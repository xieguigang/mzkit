require(buffer);

# the raw data base64 string copy from the peaks node
# of a demo mzxml file
const raw as string = "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";

let runDecode_demo as function() {

	# the steps in R# scripting for decode the ms matrix 
	# from the raw data string 
	const data_decode as double = raw
	# decode the base64 string at first for get the raw byte stream data
	:> base64_decode      
	# the demo raw bytes data stream is compressed in zlib format
	# do zlib decompression of the stream data
	:> zlib.decompress
	# then we run bit convert of the byte stream data as floats number array
	:> float(networkOrder = TRUE, sizeOf = 64)
	;

	print("demo decode of the raw MS data:");
	print(data_decode);

	# mzXML is mz-into tuple data
	# odds is m/z
	# and even is intensity value
	const mz as double   = data_decode[1:length(data_decode) step 2];
	const into as double = data_decode[2:length(data_decode) step 2];
	
	# finally we have the ms matrix
	# decoded from the mzxml file
	const ms_matrix = data.frame("m/z" = mz, intensity = into);

	cat("\n\n");

	print("the decoded ms matrix output from the raw data:");
	print(ms_matrix);
	
	ms_matrix;
}

runDecode_demo();

