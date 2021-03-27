require(netCDF.utils);

using cdf as open.netCDF("P:\MTBLS212\CZ1_5m.cdf") {
	print("all of the global attributes in cdf data file:");
	print(globalAttributes(cdf));
	
	print("all of the data variables in cdf data file:");
	print(variables(cdf));
	pause();
	print(var(cdf, "") :> getValue);
}