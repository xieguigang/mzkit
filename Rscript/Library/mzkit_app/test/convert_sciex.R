require(mzkit);

# controls of the server memory usage in the data analysis workflow.
options(memory.load = "max");

#' biodeep mzweb data viewer raw data file helper
imports "mzweb" from "mzkit";
#' raw data accessor for the mzpack data object
imports "mzPack" from "mzkit";

const rawfile as string = ?"--raw";
const savefile = `${dirname(rawfile)}/${basename(rawfile)}.mzPack`;

print(rawfile);
print(savefile);

rawfile
|> mzweb::open.mzpack()
# |> mzPack::removeSciexNoise()
|> mzweb::write.mzPack(file = savefile, version = 1)
;