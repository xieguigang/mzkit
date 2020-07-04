# a helper module for read sciBASIC.NET general signal netcdf4 data file.

readAllSignals = function(cdf, verbose = TRUE) {
	library(ncdf4);
	
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
	
	signals = lapply(axis, function(entry) {
		attrs = ncatt_get(nc, entry);
		signal = attrs$signal;
		
		x =  ncvar_get(nc, entry);
		y =  ncvar_get(nc, signal);
		
		attrs = append( ncatt_get(nc, signal), attrs);
		
		list(id = signal, attrs = attrs, signal = data.frame(axis = x, signal = y));
	});
	
	names(signals) = sapply(signals, function(i) i$id);
	signals;
}