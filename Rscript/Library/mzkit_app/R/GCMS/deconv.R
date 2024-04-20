let deconv_gcms = function(rawdata, export_dir = "./", peak.width = [3, 90], n_threads = 16) {
    let files = list.files(rawdata, pattern = "*.mzPack");
    let peaks_export = file.path(export_dir, "peaksdata");
    let decode_gcms = function(filepath) {
        require(mzkit);

        filepath 
        |> __deconv_gcms_single(peak.width)
        |> writeBin(con = file.path(peaks_export, `${basename(filepath)}.peakdata`))
        ;
    }

    # extract peaks data from rawdata files
    parallel(path = as.list(files, names = basename(files)), n_threads = n_threads, 
                ignoreError = TRUE) {

        # get peak features and corresponding spectrum 
        # data for each single sample data file.
        decode_gcms(path);
    }

    # load peaks features, and then merge as a peaktable
    let peakstable = peaks_export 
    |> list.files(pattern = "*.peakdata")
    |> lapply(path -> readBin(path, what = "gcms_peak"), names = path -> basename(path))
    |> peak_alignment()
    ;
    let rt_shifts = attr(peakstable, "rt.shift");

    write.csv(peakstable, file = `${export_dir}/peaktable.csv`, 
        row.names = TRUE);
    write.csv(rt_shifts, file = `${export_dir}/rt_shifts.csv`, 
        row.names = TRUE);

    svg(file = file.path(export_dir, "rt_shifts.svg")) {
        plot(rt_shifts, res = 1000);
    }

    invisible(NULL);
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