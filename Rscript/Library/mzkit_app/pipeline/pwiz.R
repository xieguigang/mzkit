imports "Parallel" from "snowFall";

[@info "A directory path that should contains at least 1 raw data file."]
[@type "directory"]
const data as string = ?"--data" || stop("a directory path that contains raw data files must be provided!");
const raws as string = `./${basename(list.files(data, pattern = "*.raw"))}.raw`;

print("get a list of raw data files:");
print(raws);

print("run pwiz raw to mzML in parallel!");

parallel(raw = raws, n_threads = 8, debug = TRUE) {
   [@ioredirect FALSE]
    @`docker run -i --rm -e WINEDEBUG=-all 
        -v "$PWD:/data" 
        -w "/data" 
        chambm/pwiz-skyline-i-agree-to-the-vendor-licenses 
        wine msconvert ${raw}
    `;
}