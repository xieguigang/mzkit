imports "mzweb" from "mzkit";

const file as string = ?"--raw" || stop("no raw data file location was specific!");

# run process of current single file
mzweb::packBin(file, `${dirname(file)}/${basename(file)}.mzPack`);