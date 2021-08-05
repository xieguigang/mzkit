#' Read the netcdf file
#' 
#' @description Read the netcdf file that contains the MS data converted
#'    from the mzPack file format.
#' 
#' @param cdf the file path of the netCDF data file.
#' 
#' @details the mzpack data write to netcdf file can be found at source file module:
#' 
#'    
#' 
readMzPackCDF = function(cdf, verbose = TRUE) {
    library(ncdf4);

    nc <- nc_open(cdf);

    if (verbose) {
        print(paste("The file [", cdf ,"]has",nc$nvars,"variables,",nc$ndims,"dimensions and",nc$natts,"NetCDF attributes"));
    }

    
}