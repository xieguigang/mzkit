#' Read the predict result file from cfm-id EI model output
#' 
const read.cfmid_3_EI = function(file) {
    let data = readLines(file) |> split(s -> nchar(s) == 0);
    let spectral = as.character(data[[1]]);
    let fragments = as.character(data[[2]]);

    spectral = spectral |> shift(fill=NULL) |> lapply(s -> as.numeric(strsplit(s,"\s+")));
    spectral = peakMs2(0,0, mz =spectral@{1}, into = spectral@{2});

    fragments = fragments |> shift(fill=NULL) |> lapply(s -> strsplit(s,"\s+"));
    fragments = data.frame(
        mz = as.numeric( fragments@{2}),
        annotation = fragments@{3}
    );

    list(spectral, annotations = fragments);
}