require(xcms);

#' Load the mzXML rawdata file and the export as R binary data file
#'
#' @param file the file path to a single mzXML rawdata file.
#'
load_ms2 = function(file) {
	# construct the data save path
	savefile = sprintf("%s.RData", normalizePath(file));

	# read rawdata file and extract the ms2 level peaks data inside
	xraw = xcmsRaw(file, includeMSn = TRUE);
	xms2 = msn2xcmsRaw(xraw);
	rt2  = xms2@scantime;
	mz1  = xms2@msnPrecursorMz;
    into = xms2@msnPrecursorIntensity;
	xcms_id = sprintf("M%sT%s, %s@%smin", round(mz1), round(rt2), round(mz1, 4), round(rt2 / 60, 1));
	# load each ms2 scan data from the rawdata file
	ms2  = lapply(1:length(rt2), function(i) {
		getScan(xms2, i);
	});
	
	# then save the ms2 level spectrum data as R dataset
	names(ms2) = make.unique(xcms_id);
	save(mz1, rt2, into, ms2, file = savefile);
	
	invisible(NULL);
}

files = list.files("./raw", pattern = ".+.mzXML");

# loop throught each rawdata file for save as binary data
for(file in files) {
	load_ms2(file);
}