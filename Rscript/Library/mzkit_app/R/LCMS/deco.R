imports "mzDeco" from "mz_quantify";
imports "mzweb" from "mzkit";
imports "Parallel" from "snowFall";
imports ["data","math"] from "mzkit";
imports "visual" from "mzplot";

#' Do ms1 deconvolution of the rawdata
#' 
#' @param rawdata a directory path that contains the mzXML or mzML rawdata files.
#' @param outputdir a directory path for save the peaktable result file and the
#'     temp cache files.
#' @param mzdiff the mass tolerance error for create the mz bins from the ms1 
#'     rawdata, the XIC data is generated based on this parameter value
#' @param peak.width the rt range of the peak data
#' @param n_threads the cpu thread number for apply for the peak alignments when export peaktable data.
#' @param xic_mzdiff the mass tolerance error for make extract of the xic data 
#'     from all of the ms1 rawdata scatters. 
#' 
#' @param filename the save file name of the exports generated peaktable file.
#' 
#' @return this function returns nothing 
#' 
const run.Deconvolution = function(rawdata, outputdir = "./", mzdiff = 0.01, xic_mzdiff = 0.005,
                                   peak.width = [2, 30], n_threads = 16, 
                                   top_n = 20000,
                                   args = list(
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
                                   filename = "peaktable.csv") {
                                    
    const xic_cache = `${outputdir}/XIC_data`;
    const files = 
    {
        if (dir.exists(rawdata)) {
            # scan dir for files
            list.files(rawdata, pattern = ["*.mzML", "*.mzXML", "*.mzPack"]);
        } else {
            rawdata;
        }
    }

    print("run deconvolution for rawdata files:");
    print(basename(files));

    options(n_threads = n_threads);

    # create temp data of ms1 XIC
    # use fixed 8 CPU threads for extract XIC from the rawdata files.
    ms1_xic_bins(files, mzdiff = xic_mzdiff, 
        outputdir = xic_cache, 
        n_threads = 8);
    
    # get xic file path list
    const xic_files = list.files(xic_cache, pattern = "*.xic");
    const ion_features_csv = file.path(outputdir, "mzbins.csv");

    # and then extract the ion features from the input xic files
    const bins = {
        if (file.exists(ion_features_csv)) {
            read.csv(ion_features_csv, row.names = NULL, check.names = FALSE);
        } else {
            print("start to extract the ion features from the XIC pool...");
            # extract ion features and dump as table
            let massSet = xic_files 
            |> ms1_mz_bins()
            ;

            massSet |> write.csv(file = ion_features_csv, 
                row.names = FALSE);
            massSet;
        }
    };    

    # the bin object just a dataframe object that with 
    # two data column:
    #
    # 1. mz
    # 2. into
    #
    let peaktable = ms1_peaktable(xic_files, bins, 
        args = args, 
        mzdiff = mzdiff,
        peak.width = peak.width, 
        tmp_out = file.path(outputdir, "peaksdata")
    );
    let rt_shifts = attr(peaktable, "rt.shift");

    # group by rt
    peaktable <- rt_groups(peaktable,
        dt = 3,
        ppm = 20) |> top_peaks(n = top_n )
    ;

    write.csv(peaktable, file = `${outputdir}/${filename}`, 
        row.names = TRUE);
    write.csv(rt_shifts, file = `${outputdir}/rt_shifts.csv`, 
        row.names = TRUE);

    let peaksarea = as.data.frame(peaktable, peaks_area = TRUE); 

    peaksarea <- apply(peaksarea, margin = "row", FUN = sum);

    print("sum peak area for each peak features:");
    print(peaksarea);

    if (length(peaktable) > 0) {
        let peakmeta = data.frame(
            mz = [peaktable]::mz, mzmin = [peaktable]::mzmin, mzmax = [peaktable]::mzmax,
            rt = [peaktable]::rt, rtmin = [peaktable]::rtmin, rtmax = [peaktable]::rtmax,
            RI = [peaktable]::RI,
            npeaks = [peaktable]::npeaks,
            into = peaksarea,
            row.names = [peaktable]::ID
        );

        print("view of the lcms peaks ROI metadata:");
        print(peakmeta, max.print = 6);

        write.csv(peakmeta, file = `${outputdir}/peakmeta.csv`, 
            row.names = TRUE);

        if (length(rt_shifts) > 1) {
            bitmap(file = file.path(outputdir, "rt_shifts.png"), size = [4000, 2700], padding = [50 650 200 200]) {
                plot(rt_shifts, res = 1000, grid.fill = "white");
            }
            pdf(file = file.path(outputdir, "rt_shifts.pdf"), size = [4000, 2700], padding = [50 650 200 200]) {
                plot(rt_shifts, res = 1000, grid.fill = "white");
            }
        }

        bitmap(file = file.path(outputdir, "peakset.png")) {
            plot(as.peak_set(peakmeta), scatter = TRUE, 
                dimension = "npeaks");
        }
        pdf(file = file.path(outputdir, "peakset.pdf")) {
            plot(as.peak_set(peakmeta), scatter = TRUE, 
                dimension = "npeaks");
        }
    }

    invisible(NULL);
} 