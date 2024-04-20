let deconv_gcms = function(rawdata, export = "./", peak.width = [3, 90], n_threads = 16) {
    let files = list.files(rawdata, pattern = "*.mzPack");
    let decode_gcms = function(file) {
        require(mzkit);

        file 
        |> __deconv_gcms_single(peak.width)
        |> writeBin(con = file.path(export, "peaksdata", `${basename(file)}.peakdata`))
        ;
    }

    parallel(path = files, n_threads = n_threads, 
                ignoreError = TRUE) {

        # get peak features and corresponding spectrum 
        # data for each single sample data file.
        decode_gcms(path);
    }
}

let __deconv_gcms_single = function(file, peak.width = [3, 90]) {
    imports "GCMS" from "mz_quantify";

    file 
    |> open.mzpack()
    |> GCMS::ROIlist(
        peakwidth = peak.width,
        baseline = 0.65,
        sn = -999,
        joint = FALSE
    );
}