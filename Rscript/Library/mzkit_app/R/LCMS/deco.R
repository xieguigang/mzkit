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
#' 
#' @return this function returns nothing 
#' 
const run.Deconvolution = function(rawdata, outputdir = "./", mzdiff = 0.005, 
                                   peak.width = [3, 90]) {
                                    
    const xic_cache = `${outputdir}/XIC_data`;
    const files = list.files(rawdata, pattern = ["*.mzML", "*.mzXML", "*.mzPack"]);

    print("run deconvolution for rawdata files:");
    print(basename(files));

    # create temp data of ms1 XIC
    ms1_xic_bins(files, mzdiff = mzdiff, 
        outputdir = xic_cache, 
        n_threads = 32);
    
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
    const peaktable = ms1_peaktable(xic_files, bins, 
        mzdiff = mzdiff, 
        peak.width = peak.width);
    const rt_shifts = attr(peaktable, "rt.shift");

    write.csv(peaktable, file = `${outputdir}/peaktable.csv`, 
        row.names = TRUE);
    write.csv(rt_shifts, file = `${outputdir}/rt_shifts.csv`, 
        row.names = TRUE);

    svg(file = file.path(outputdir, "rt_shifts.svg")) {
        plot(rt_shifts, res = 1000);
    }

    invisible(NULL);
} 