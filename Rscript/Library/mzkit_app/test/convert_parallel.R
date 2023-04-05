imports "Parallel" from "snowFall";

rawdir = ?"--dir";
files = list.files(rawdir, pattern = "*.mzXML");

parallel(rawfile = files, n_threads = 32) {
    require(mzkit);

    #' biodeep mzweb data viewer raw data file helper
    imports "mzweb" from "mzkit";
    #' raw data accessor for the mzpack data object
    imports "mzPack" from "mzkit";

    const savefile = `${dirname(rawfile)}/${basename(rawfile)}.mzPack`;

    rawfile
    |> open.mzpack()
    |> removeSciexNoise()
    |> write.mzPack(file = savefile, version = 1)
    ;
}