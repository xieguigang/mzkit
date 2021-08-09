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

  scans = names(nc$var);
  ms2i = sapply(scans, function(name) {
    meta = ncatt_get(nc, name);
    as.integer(meta$mslevel) != 1
  });

  names(scans) = scans;

  # read ms2 data
  lapply(scans[ms2i], function(name) {
    data   = ncvar_get(nc, name);
    meta   = ncatt_get(nc, name);
    n      = length(data) / 2;
    mz     = data[1:n];
    into   = data[(n+1):length(data)];
    matrix = data.frame(mz = mz, into = into);

    list(mz = meta$mz, ms2 = matrix);
  });
}
