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
    
    # extract peaks data from the XIC data
    files |> make_peak_samples(args = args, 
        simple = simple,                                   
        n_threads = n_threads, 
        tmp_out = tmp_out)
    ;
    # make assemble of the sample peak files as peaktable
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