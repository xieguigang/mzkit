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
const normData = function(mat, factor = NULL) {
	const dat = as.data.frame(mat);
	const min as double = mzkit::.minPos(dat) / 2;	
	
	if (!is.null(factor)) {
		factor = as.numeric(factor);
	} else {
		factor = colnames(dat) 
		|> lapply(function(i) dat[, i]) 
		|> unlist() 
		|> unlist() 
		|> as.numeric() 
		|> sum()
		;
	}

	for(name in colnames(dat)) {
		v = as.numeric(dat[, name]);
		v[v <= 0.0] = min;
		v = v / sum(v) * factor;
		
		dat[, name] = v;
	}
	
	dat;
}

const .minPos = function(mat) {
	let v = [];
	
	for(name in colnames(mat)) {
		v = append(v, mat[, name]);
	}
	
	v = as.numeric(v);
	v = min(v[v > 0]);
	v;
}

#' Create a dataset for evaluate ANOVA p-value 
#' 
const ANOVAGroup = function(data, sampleinfo) {
    if (is.null(data)) {
        NULL;
    } else {
        region_group = [];
        intensity = [];
        
        for(group in sampleinfo) {
            intensity = append(intensity, unlist(data[group$id]));
            region_group = append(region_group, rep(group$group, length(group$id)));
        }

        data = data.frame(intensity , region_group);
        data;
    }
}