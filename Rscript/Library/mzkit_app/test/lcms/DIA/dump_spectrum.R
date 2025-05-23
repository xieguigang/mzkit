require(mzkit);

let mz = 794.7229;
let rt = 7.28 * 60;

rt <- [rt - 7.5, rt + 7.5];

let spectrum = [];
let rawfiles = list.files("F:/test_batch1", pattern = "*.mzPack");

for(let file in tqdm(rawfiles)) {
    file <- open.mzpack(file, verbose = FALSE);
    spectrum <- append(spectrum, file |> ms2_peaks(
        precursorMz = mz,
        tolerance = "da:0.01",
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