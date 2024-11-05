require(mzkit);

let mz = 794.6042;
let rt = 7.28 * 60;

rt <- [rt - 10, rt + 10];

let spectrum = [];
let rawfiles = list.files("F:\testdata", pattern = "*.mzPack");

for(let file in tqdm(rawfiles)) {
    file <- open.mzpack(file, verbose = FALSE);
    spectrum <- append(spectrum, file |> ms2_peaks(
        precursorMz = mz,
        tolerance = "da:0.1",
        tag_source = TRUE,
        centroid = FALSE,
        norm = FALSE,
        filter_empty = TRUE,
        into_cutoff = 0,
        rt_window = rt
    ));
}

spectrum <- centroid(spectrum, tolerance = "da:0.3");

write.cache(spectrum, file = "F:/testdata.spec");