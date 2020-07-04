# a helper module for read sciBASIC.NET general signal netcdf4 data file.

readAllSignals = function(cdf, n_threads = 4, verbose = TRUE) {
	library(ncdf4);
	
	require(foreach);
    require(doParallel);
	
	Imports(System.Text.RegularExpressions);
	
	nc <- nc_open(cdf);
	
	if (verbose) {
		print(paste("The file has",nc$nvars,"variables,",nc$ndims,"dimensions and",nc$natts,"NetCDF attributes"));
	}
	
	axis = attributes(nc$var)$names;
	axis = axis[axis %in% Matches(axis, "axis\\d+")];
	
	if (verbose) {
		print(sprintf("loading %s signals data...", length(axis)));
	}
	
    cl <- makeCluster(n_threads);
    registerDoParallel(cl);
  
	signals = foreach(entry = axis, .verbose = FALSE) %dopar% {
		library(ncdf4);
	
		attrs = ncatt_get(nc, entry);
		signal = attrs$signal;
		
		x =  ncvar_get(nc, entry);
		y =  ncvar_get(nc, signal);
		
		attrs = append( ncatt_get(nc, signal), attrs);
				
		list(id = signal, attrs = attrs, signal = data.frame(axis = x, signal = y));
	};
	
	stopCluster(cl);
	nc_close(nc);
	
	names(signals) = sapply(signals, function(i) i$id);
	signals;
}