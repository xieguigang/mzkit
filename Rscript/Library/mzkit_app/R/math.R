imports "math" from "mzkit";

#' Create mzdiff tolerance value
#' 
#' @param kind da method or ppm method.
#' @param mzdiff the m/z tolerance error value.
#' 
const tolerance = function(kind as string, mzdiff as double) {
    math::tolerance(mzdiff, kind);
}

#' Normalize matrix sample data
#'
#' @return a result data matrix has been normalized 
#'     via total sum of the peak area.
#'
const normData = function(mat, factor = 10^12) {
	const min = mzkit::.minPos(mat) / 2;
	const dat = as.data.frame(mat);
	
	v = [];
	
	for(name in colnames(mat)) {
		v = append(v, mat[, name]);
	}
	
	tpa = sum(as.numeric(v));
	
	for(name in colnames(mat)) {
		v = as.numeric(mat[, name]);
		v[v == 0.0] = min;
		v = v / tpa * factor;
		
		mat[, name] = v;
	}
	
	mat;
}

const .minPos = function(mat) {
	v = [];
	
	for(name in colnames(mat)) {
		v = append(v, mat[, name]);
	}
	
	v = as.numeric(v);
	v = min(v[v > 0]);
	v;
}