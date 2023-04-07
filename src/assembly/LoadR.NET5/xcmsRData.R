load_ms2 = function(file) {
	require(xcms);
	
	xraw = xcmsRaw(file, includeMSn = TRUE);
	xms2 = msn2xcmsRaw(xraw);
	rt2  = xms2@scantime;
	mz1  = xms2@msnPrecursorMz;
    into = xms2@msnPrecursorIntensity;
	xcms_id = sprintf("M%sT%s, %s@%smin", round(mz1), round(rt2), round(mz1, 4), round(rt2 / 60, 1));
	
	ms2  = lapply(1:length(rt2), function(i) {
		getScan(xms2, i);
	});
	names(ms2) = make.unique(xcms_id);
	
	savefile = sprintf("%s.RData", normalizePath(file));
	
	save(mz1, rt2, into, ms2, file = savefile);
}