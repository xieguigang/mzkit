library("mzkit/mzPack");

using mzpack as mzpack(file("F:\TEMP\mzkit_win32\MSI_imzML\c13779f1-6cbb-7e40-85ae-cfad49ba095d")) {
	mzpack
	|> mzPack::ls()
	|> print
	;
	
	str(mzpack |> metadata(mzPack::ls(mzpack)[1]));
	
	const ms1_scan = mzpack |> scaninfo(mzPack::ls(mzpack)[1]);
	const matrix   = data.frame(
		"m/z"       = ms1_scan$mz, 
		"intensity" = ms1_scan$into
	);
	
	str(ms1_scan);
	
	print("MS1 matrix:");
	print(matrix);
} 
