#' Create XIC rawdata file for run peaktable exports
#' 
#' @param files the LCMS rawdata files, could be in format of
#'    mzXML or mzML.
#' @param mzdiff a numeric vector for create ion m/z feature bins
#' @param outputdir a character vector of local filesystem directory 
#'    path for save the XIC temp result data files.
#' @param n_threads thread number for read the rawdata files. 
#' 
#' @return this function returns nothing
#' 
const ms1_xic_bins = function(files, mzdiff = 0.005, 
                              outputdir = "./XIC/", 
                              n_threads = 32) {

    let cache_exists = function(file, min_size, cache.enable = TRUE) {
        if (nchar(file) == 0) {
            FALSE;
        } else {
            if (file.size(file) < min_size) {
                FALSE;
            } else {
                cache.enable;
            }
        }
    }

    let xic_cache = `${outputdir}/${basename(files)}.xic`;
    let i = sapply(xic_cache, cache -> cache_exists(cache, 
        min_size = 1*1024*1024, cache.enable = TRUE)
    );

    print(`run XIC rawdata exports for ${length(files)} rawdata files:`);
    print(basename(files));   
    
    if (any(!i)) {
        # the source rawdata files
        # skip of the existsed cache files, make subset of the rawdata files 
        # which has no cached
        files <- files[!i];
        files <- as.list(files, names = basename(files));

        print(`processing ${length(files)} un-cahced rawdata files:`);
        str(files);

        parallel(path = files, n_threads = n_threads, 
                 ignoreError = TRUE) {
                    
            require(mzkit);

            # processing a single rawdata file
            # extract XIC data for run the downstream peak detection.
            __ms1_xic_bins_single(
                path = unlist(path), mzdiff = unlist(mzdiff), 
                outputdir = unlist(outputdir));
        }
    }

    invisible(NULL);
}

#' export XIC data for a single rawdata file
#' 
#' @param path a single rawdata file its file path, should be in mzpack file format.
#' @param mzdiff the mass tolerance error in delta dalton for extract the
#'    xic values from the ms1 data.
#' @param outputdir a directory for save the temp file
#' 
#' @return this function returns nothing
#' 
const __ms1_xic_bins_single = function(path, mzdiff, outputdir) {
    let xic_cache = `${outputdir}/${basename(path)}.xic`;

    if (file.size(xic_cache) > (1*1024*1024)) {
        # has cache file
        # do nothing
        print(`skip cached xic: ${basename(path)}`);
    } else {
        let rawdata = open.mzpack(path);
        let xic = mz.groups(ms1 = rawdata, 
            mzdiff = `da:${mzdiff}`);

        writeBin(xic, con = xic_cache);
    }
}