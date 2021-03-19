imports "mzweb" from "mzkit";

# demo test of open an mzPack binary file
const file as string = ?"--raw" || stop("no raw data file location was specific!");
const mzpack = open.mzpack(file);


