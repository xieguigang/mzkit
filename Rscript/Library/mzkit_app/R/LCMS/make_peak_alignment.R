
#' make peaktable
#' 
#' @param peakfiles a character vector of the peakdata files of each sample files
#' 
const make_peak_alignment = function(peakfiles, max_rtwin = 15,mzdiff = 0.01) {
    imports "xcms" from "mzkit";

    let peaksdata = as.list(peakfiles, names = basename(peakfiles)) 
    |> tqdm() 
    |> lapply(function(path) {
        path 
        |> read.csv(row.names = NULL, 
            check.names = FALSE
        ) 
        |> cast_findpeaks_raw(basename(path))
        ;
    })
    ;

    peaksdata |> peak_alignment( 
        mzdiff = mzdiff,
        ri_win = max_rtwin);
}