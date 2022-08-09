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

#' Find the min positive value in the given dataset
#' 
#' @param mat a dataset object that could be a 
#'     dataframe or a list data object.
#' 
#' @return a numeric value that is the min positive
#'     value in the mat data input.
#' 
const .minPos = function(mat) {
	let v = [];
	
	if (typeof(mat) == "list") {
		v = unlist(mat);
	} else {
		for(name in colnames(mat)) {
			v = append(v, mat[, name]);
		}
	}
	
	v = as.numeric(v);
	v = min(v[v > 0]);
	v;
}

#' Create a dataset for evaluate ANOVA p-value 
#' 
#' @param data a data list object that should contains 
#'     group sample raw data of the target metabolite 
#'     ion. the list key name is the sample id and the 
#'     list element data is the corresponding sample 
#'     intensity data.
#' 
#' @param sampleinfo a list data that should contains 
#'     the data sample group tag information. the data 
#'     structre of this list could be two fields are 
#'     required at least:
#'         
#'       + group: the sample group name
#'       + id: a character vector that contains the sample 
#'             id to get intensity vector data from the 
#'             ``data`` parameter.
#' 
#' @return a dataframe object that contains two fields:
#'     ``intensity`` and ``region_group``, which could be 
#'     used for do stat chartting plot liked bar/box/violin
#'     and also could be used for evaluated ANOVA F-test 
#'     p-value result.
#' 
const ANOVAGroup = function(data, sampleinfo) {
    if (is.null(data)) {
        NULL;
    } else {
        let region_group as string = [];
        let intensity as double = [];
        
        for(group in sampleinfo) {
            intensity = append(intensity, unlist(data[group$id]));
            region_group = append(region_group, rep(group$group, length(group$id)));
        }

        data = data.frame(intensity, region_group);
        data;
    }
}