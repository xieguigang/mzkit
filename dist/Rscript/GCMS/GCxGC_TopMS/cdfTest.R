require(netCDF.utils);

using cdf as open.netCDF("P:\MTBLS212\CZ1_5m.cdf") {
	print("all of the global attributes in cdf data file:");
	print(globalAttributes(cdf));
	
	print("all of the data dimensions:");
	print(dimensions(cdf));
	
	print("all of the data variables in cdf data file:");
	print(variables(cdf));

	print("a_d_sampling_rate");
	print(var(cdf, "a_d_sampling_rate") :> getValue);
	
	print("a_d_coaddition_factor");
	print(var(cdf, "a_d_coaddition_factor") :> getValue);
	
	print("scan_acquisition_time");
		
	const scan_time = var(cdf, "scan_acquisition_time") :> getValue;
	
	print(scan_time);
	# scan_time :> writeLines(con = "D:/testaaaa.csv");
	
	print("scan_duration");
	print(var(cdf, "scan_duration") :> getValue);
	
	print("inter_scan_time");
	print(var(cdf, "inter_scan_time") :> getValue);
	
	print("resolution");
	print(var(cdf, "resolution") :> getValue);
	
	print("actual_scan_number");
	print(var(cdf, "actual_scan_number") :> getValue);
	
	print("total_intensity");
		
	const into = var(cdf, "total_intensity") :> getValue;
	
	print(into);
	
	data.frame(scan_time, into) :> write.csv(file = "D:\testaaaa.csv", row_names = FALSE);
	
	print("mass_range_min");
	print(var(cdf, "mass_range_min") :> getValue);
	
	print("mass_range_max");
	print(var(cdf, "mass_range_max") :> getValue);
	
	print("time_range_min");
	print(var(cdf, "time_range_min") :> getValue);
	
	print("time_range_max");
	print(var(cdf, "time_range_max") :> getValue);
	
	print("scan_index");
	print(var(cdf, "scan_index") :> getValue);
	
	print("point_count");
	print(var(cdf, "point_count") :> getValue);
	
	print("flag_count");
	print(var(cdf, "flag_count") :> getValue);
	
}