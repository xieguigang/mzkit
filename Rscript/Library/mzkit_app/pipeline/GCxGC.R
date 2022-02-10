require(mzkit);

#' workflow script for processing GCxGC raw data file
#' 
#' 

imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";

[@info "the file path of the GCxGC raw data file, or a folder path that contains multiple cdf raw data files."]
cdfpath   = ?"--cdf"       || stop("no cdf file provided!");
outputdir = ?"--outputdir" || `${dirname(cdfpath)}/processed/`;
[@info "GCxGC modtime in time unit seconds."]
modtime   = ?"--modtime"   || 5;

#' Process cdf raw data file
#' 
#' @param cdf_src the file path of the GCxGC cdf raw data file.
#' 
processCdfFile = function(cdf_src) {

}