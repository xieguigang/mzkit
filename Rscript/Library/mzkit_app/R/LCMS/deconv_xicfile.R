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

    if (file.exists(peakfile ) && file.exists(peakdata )) {
        message(`[${rawfile}] cache existed, skip of the peak finding process`);
    } else {
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
}