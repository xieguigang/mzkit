imports "mzweb" from "mzkit";

const rawdir as string = ?"--rawdir" || stop("no raw data location was specific!");

for(file in list.files(rawdir, pattern = "*.mz*ML")) {
	print(file);
	
	const packFile = `${dirname(file)}/${basename(file)}.mzPack`;
	
	if (!file.exists(packFile)) {
		# run process of current single file
		mzweb::packBin(file, packFile);
	}
} 