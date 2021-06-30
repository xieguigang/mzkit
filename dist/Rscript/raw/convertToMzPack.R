imports "mzweb" from "mzkit";

const rawfile as string = ?"--raw" || stop("the file path of the raw data file is required!");
const outfile as string = ?"--out" || `${dirname(rawfile)}/${basename(rawfile)}.mzPack`;

rawfile
|> open.mzpack
|> write.mzPack(file = outfile)
;