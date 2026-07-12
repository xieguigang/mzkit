const make_peak_samples = function(files, args = list(
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
                                   n_threads = 32, 
                                   tmp_out = "./tmp") {
                                    
    let xic_args = as.list(args);

    message("inspect of the parameter:");
    str(args);
    
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

    invisible(NULL);
}