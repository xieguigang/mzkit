imports "mzweb" from "mzkit";

const rawdir as string = ?"--rawdir" || stop("no raw data location was specific!");

for(file in list.files(rawdir, pattern = "*.mz*ML")) {
	print(file);
	
	# run process of current single file
	mzweb::packBin(file, `${dirname(file)}/${basename(file)}.mzPack`);
} 