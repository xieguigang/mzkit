imports "mzDeco" from "mz_quantify";

#' Export peakstable data from the XIC rawdata files
#' 
#' @param files a character vector of the file path of the XIC rawdata files
#' @param mzbins A dataframe object that contains the target ion m/z feature 
#'    set for do extract of the result peaktable data, a data field which is 
#'    named ``mz`` must be included inside this dataframe object.
#' @param peak.width the peak time range of the peaks that could be accepted
#' 
#' @return this function generates a xcms format liked peaktable dataframe object
#'    for the input rawdata files.
#' 
const ms1_peaktable = function(files, mzbins, mzdiff = 0.01,
        args = list(
            # peak finding and calculate peak area
            peak_method = "CentWave",
            snr_threshold = 3.0,
            window_half_width = 5,
            min_peak_width = 3.0,
            max_peak_width = 30.0,
            min_peak_height = 0.0,
            centWave_min_scale = 1,
            centWave_max_scale = 20,
            centWave_scale_step = 1,
            centWave_max_gap = 2,
            matched_filter_sigma = 3.0,
            matched_filter_truncate_width = 4.0,
            derivative_smooth_window = 3,
            derivative_threshold_factor = 0.01,
            noise_segment_count = 20,
            peak_merge_distance = 1.0,
            area_method = "BaselineCorrected",
            baseline_method = "Linear",
            baseline_percentile = 10.0,
            local_minimum_boundary_points = 5,
            gaussian_max_iterations = 100,
            gaussian_convergence = 0.000001,
            recalculate_snr = TRUE
        ), 
        simple = TRUE,
        method = "LOESS",
        n_threads = 32, 
        tmp_out = "./tmp") {

    let xic_args = as.list(args);

    message("inspect of the parameter:");
    str(args);

    mzbins = mzkit::mz_bin_features(mzbins);
    
    # ------------------------ RUN Parallel --------------------------------
    Parallel::parallel(raw_path = files, n_threads = n_threads, 
                ignoreError = FALSE, 
                debug = FALSE,
                log_tmp = `${tmp_out}/.local_debug/`,
                compress = FALSE) {
                    
        require(mzkit);

        mzkit::deconv_xicfile(
            path = unlist(raw_path), 
            mzbins = NULL, 
            args = xic_args, 
            tmp_out = tmp_out,
            simple = simple
        );
    };
    # ------------------------ END Parallel --------------------------------

    align_peaktable(peaks_dir = file.path(tmp_out, "peaks"),
        mzdiff =  mzdiff, 
        method = method 
    ); 
}

const align_peaktable = function(peaks_dir = "./peaks", mzdiff = 0.01, method = "LOESS") {
    let peaksdata = list.files(peaks_dir, pattern = "*.dat");

    peaksdata = as.list(peaksdata, names = basename(peaksdata));
    peaksdata = lapply(peaksdata, filepath => readBin(filepath, what = "peak_feature"));

    # let pool = xic_pool(files);  
    # return(mz_deco(
    #     pool,                       # the XICPool raw data object 
    #     tolerance = `da:${mzdiff}`, # mass tolerance value for matches XIC with the given mzbins features
    #     joint = TRUE,               # merge the closed peaks?
    #     peak.width = peak.width,    # [min,max] peak width range
    #     feature = mzbins,           # a numeric vector of the target m/z values for extract peaks features from the XIC data
    #     parallel = TRUE)
    # );

    peaksdata |> peak_alignment(
        mzdiff = mzdiff,
        rt_win = 30,
        ri_win = 10,
        norm = FALSE,
        ri_alignment = FALSE,
        max_intensity_ion = FALSE,
        native_alignment = FALSE,
        aggregate = "Sum",
        tolerance_mode = "Da",
        method = method,
        loess_span = 0.75,
        loess_degree = 2,
        reference_sample = "",
        density_bandwidth = 0.0,
        min_fraction = 0.25,
        obiwarp_bin_size = 1.0,
        obiwarp_gap_penalty = 0.6,
        obiwarp_response = 100,
        fill_gaps = TRUE
    );
}

const deconv_xicfile = function(path, mzbins = NULL, args = list(
        peak_method = "CentWave",
        snr_threshold = 3.0,
        window_half_width = 5,
        min_peak_width = 3.0,
        max_peak_width = 30.0,
        min_peak_height = 0.0,
        centWave_min_scale = 1,
        centWave_max_scale = 20,
        centWave_scale_step = 1,
        centWave_max_gap = 2,
        matched_filter_sigma = 3.0,
        matched_filter_truncate_width = 4.0,
        derivative_smooth_window = 3,
        derivative_threshold_factor = 0.01,
        noise_segment_count = 20,
        peak_merge_distance = 1.0,
        area_method = "BaselineCorrected",
        baseline_method = "Linear",
        baseline_percentile = 10.0,
        local_minimum_boundary_points = 5,
        gaussian_max_iterations = 100,
        gaussian_convergence = 0.000001,
        recalculate_snr = TRUE
    ), 
        simple = TRUE, tmp_out = "./") {

    let rawfile = basename(path);
    let xicdata = readBin(path, what = "mz_group", mz = mzbins, da = 0.025);
    let peaks = NULL;
    let peakfile = file.path(unlist(tmp_out), "peaks", `${rawfile}.csv`);
    let peakdata = file.path(unlist(tmp_out), "peaks", `${rawfile}.dat`);

    simple = as.logical(unlist(simple));

    message("just run simple peak finding?");
    str(simple);

    for(let mz_xic in xicdata) {
        peaks = c(peaks, {
            if (simple) {
                find_peaks.simple(
                    x = mz_xic,
                    peak_width = c(args$min_peak_width, args$max_peak_width),
                    snr_threshold = 0, # args$snr_threshold,
                    filename = rawfile,
                    joint = FALSE,
                    interpolate = FALSE
                );
            } else {
                find_peaks(
                    x = mz_xic,
                    peak_method = args$peak_method,
                    snr_threshold = args$snr_threshold,
                    window_half_width = args$window_half_width,
                    min_peak_width = args$min_peak_width,
                    max_peak_width = args$max_peak_width,
                    min_peak_height = args$min_peak_height,
                    centWave_min_scale = args$centWave_min_scale,
                    centWave_max_scale = args$centWave_max_scale,
                    centWave_scale_step = args$centWave_scale_step,
                    centWave_max_gap = args$centWave_max_gap,
                    matched_filter_sigma = args$matched_filter_sigma,
                    matched_filter_truncate_width = args$matched_filter_truncate_width,
                    derivative_smooth_window = args$derivative_smooth_window,
                    derivative_threshold_factor = args$derivative_threshold_factor,
                    noise_segment_count = args$noise_segment_count,
                    peak_merge_distance = args$peak_merge_distance,
                    area_method = args$area_method,
                    baseline_method = args$baseline_method,
                    baseline_percentile = args$baseline_percentile,
                    local_minimum_boundary_points = args$local_minimum_boundary_points,
                    gaussian_max_iterations = args$gaussian_max_iterations,
                    gaussian_convergence = args$gaussian_convergence,
                    recalculate_snr = args$recalculate_snr,
                    as_peaks = TRUE,                               
                    filename = rawfile
                );
            }
        });
    }

    # peaks = as.vector(peaks);

    message(`get ${length(peaks)} feature peaks!`);

    write_peaks(peaks, file = peakdata);
    write.csv(as.data.frame(peaks), file = peakfile);
}

const mz_bin_features = function(mzbins) {
    print("get m/z bins input:");
    str(mzbins);

    if (is.character(mzbins)) {
        mzbins = read.csv(mzbins, row.names = NULL, check.names = FALSE);
        mzbins = mzbins$mz;
    } else {
        if (is.data.frame(mzbins)) {
            mzbins = mzbins$mz;
        } else {
            mzbins = as.numeric(mzbins);
        }
    }

    print("get mz ion features of the dataset:");
    print(mzbins, max.print = 13);

    print(`run extract peaktable set from ${length(mzbins)} ion m/z features...`);

    return(mzbins);
}