require(mzkit);

#' biodeep mzweb data viewer raw data file helper
imports "mzweb" from "mzkit";
#' raw data accessor for the mzpack data object
imports "mzPack" from "mzkit";

const rawfile = ?"--raw";
const savefile = `${dirname(rawfile)}/${basename(rawfile)}.mzPack`;

rawfile
|> open.mzpack()
|> removeSciexNoise()
|> write.mzPack(file = savefile, version = 1)
;