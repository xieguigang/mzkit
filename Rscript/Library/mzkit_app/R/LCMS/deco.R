imports "mzDeco" from "mz_quantify";
imports "mzweb" from "mzkit";
imports "Parallel" from "snowFall";
imports ["data","math"] from "mzkit";

#' extract peak ms1 from a set of raw data files
#' 
#' @param data_dir a directory path that contains multiple raw data files
#' 
const run.Deconvolution = function(data_dir = "./", 
                                   mzdiff = "da:0.001", 
                                   baseline = 0.65,
                                   peakwidth = [3,20],
                                   outputdir = "./", 
                                   n_threads = 8) {
    const args = list(
        cache_dir = `${normalizePath(outputdir)}/.cache/`,
        mzdiff = mzdiff,
        baseline = baseline,
        peakwidth = as.numeric(peakwidth)
    );
    const fileset = list.files(data_dir, 
        pattern = "(.+.mzX?ML)|(.+.mzPack)", 
        recursive = FALSE,
        wildcard = FALSE
    ) |> lapply(x -> x, names = x -> basename(x, withExtensionName = TRUE))
    ;
    const peakcache = `${normalizePath(outputdir)}/.cache/peaks/`;

    Parallel::parallel(rawfile = fileset, 
                       n_threads = n_threads, 
                       ignoreError = FALSE,
                       log_tmp = `${normalizePath(outputdir)}/.cache/parallel/`) {

        require(mzkit);
        mzkit::.MS1deconv(rawfile, args);
    };

    let peakdata = NULL;
    let peakfile = NULL;

    for(file in list.files(peakcache, pattern = "*.csv")) {
        peakfile = read.csv(file, row.names = NULL);
        peakfile[, "source"] = basename(file);
        peakdata = rbind(peakdata, peakfile);
    }

    peakdata[, "xcms_id"] = make.ROI_names(list(
        mz = peakdata$mz, 
        rt = peakdata$rt
    ));
    peakdata;
}

#' a single thread task for extract peaktable from a single raw data file
#' 
const .MS1deconv = function(rawfile, args = list(cache_dir = "./.cache/")) {
    const packCache = `${args$cache_dir}/raw/${basename(rawfile)}.mzPack`;
    const peakCache = `${args$cache_dir}/peaks/${basename(rawfile)}.csv`;

    if (!file.exists(packCache)) {
        rawfile 
        |> open.mzpack() 
        |> write.mzPack(file = packCache, version = 1)
        ;
    }

    const raw = packCache |> open.mzpack();
    const ms1raw = ms1_scans(raw);
    const peaks = ms1raw |> mz.deco(
        tolerance = args$mzdiff, 
        baseline = args$baseline, 
        peakwidth = args$peakwidth
    );

    write.csv(peaks, file = peakCache);
} 