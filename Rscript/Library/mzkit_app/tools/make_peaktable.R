require(mzkit);

#' title: Tool script for make peaktable from a given set of the LCMS untargetted rawdata files.
#' author: xieguigang <gg.xie@bionovogene.com>
#' description: This tool script works for a set of the rawdata files processing from MS1 rawdata to aligned peaktable file. 
#'    The export data files of this script includes the peaktable csv table, and images files for make visualization of 
#'    the RT shifts across the multipl rawdata files. 
#'    This tool script could be running on both windows and linux system which has R# programming language 
#'    environment and mzkit package installed. A ubuntu system based docker image with mzkit package installed is provided
#'    on docker hub for demonstrate the LC-MS rawdata processing of mzkit package.
#'    
#'    get docker image: docker pull xieguigang/mzkit:v20240831
#'    get mzkit source code: https://github.com/xieguigang/mzkit 

[@info "A directory path that contains the LC-MS rawdata files, 
        file format of the rawdata files must be mzXML or mzML 
        file. This parameter is required."]
let rawdata_dir = ?"--raw_dir" || stop("A directory path that contains the LC-MS mzXML/mzML rawdata files is required!");

[@info "A directory path that used for exports the result files. 
        This folder path parameter could be omit, default use the 
        sample directory location as the input rawdata directory 
        location."]
let output_dir = ?"--out_dir" || rawdata_dir;

[@info "The ion mass error window for extract the XIC from each 
        rawdata files. Default mass window is 0.01da."]
let xic_da as double = ?"--xic_da" || 0.01;

[@info "The ion mass error window for make peak groups when exports 
        the peaktable file. Default mass window is 0.01da."]
let mzdiff as double = ?"--mzdiff" || 0.01;

[@info "The peak width range in time unit seconds, should be a tuple 
        integer value that defines the min rt window width and max 
        rt window width."]
let peak_width = ?"--peak_width" || "2,12";

[@info "The thread used for make peak alignment. Default use 16 CPU threads."]
let n_threads as integer = ?"--n_threads" || 8;

options(n_threads = n_threads);

peak_width <- as.numeric(trim(unlist(strsplit(peak_width, ","))));

mzkit::run.Deconvolution(rawdata = rawdata_dir, 
    outputdir = output_dir, 
    mzdiff = mzdiff, xic_mzdiff = xic_da,
    peak.width = peak_width, 
    n_threads = n_threads);

print("~job done!");