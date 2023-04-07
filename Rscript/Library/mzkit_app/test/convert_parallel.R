imports "Parallel" from "snowFall";

rawdir = ?"--dir";
files = list.files(rawdir, pattern = "*.mzXML");

parallel(rawfile = files, n_threads = 32) {
    # require(mzkit);

    # #' biodeep mzweb data viewer raw data file helper
    # imports "mzweb" from "mzkit";
    # #' raw data accessor for the mzpack data object
    # imports "mzPack" from "mzkit";

    # const savefile = `${dirname(rawfile)}/${basename(rawfile)}.mzPack`;

    # rawfile
    # |> mzweb::open.mzpack()
    # |> mzPack::removeSciexNoise()
    # |> mzweb::write.mzPack(file = savefile, version = 1)
    # ;
    @`E:\GCModeller\src\R-sharp\App\net6.0\R#.exe E:\mzkit\Rscript\Library\mzkit_app\test\convert_sciex.R --raw "${rawfile}"`;
}