imports "math" from "mzkit";
imports "math" from "mz_quantify";

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
#' @param sampleinfo the sample information for the missing impute. this parameter 
#'     value could be a file path to the sampleinfo dataframe file, or a dataframe 
#'     object that contains the required fields for the sample info: 
#'     
#'     + ``id`` is the sample id and used for the sample file name association; 
#'     + ``name`` is the sample name and used for the sample name display; 
#'     + ``group`` is the sample group information and used for the missing row data 
#'                 impute and data filter in the processing.
#' 
#' @return a result data matrix has been normalized 
#'     via total sum of the peak area.
#'
const preprocessing_expression = function(x, sampleinfo = NULL, factor = 1e8, missing = 0.5) {
	if (!is.empty(sampleinfo)) {
		# use gcmodeller package module
		imports "sampleInfo" from "phenotype_kit";

		if (is.character(sampleinfo)) {
			sampleinfo <- read.sampleinfo(sampleinfo, 
				tsv = file.ext(sampleinfo) == "txt");
		} else {
			# may be cast from dataframe
			if (is.data.frame(sampleinfo)) {
				let id = {
					if ("id" in sampleinfo) {
						sampleinfo$id;
					} else {
						rownames(sampleinfo);
					}
				}

				# just needs the sample id and sample group information
				# for check of the missing value
				sampleinfo <- sampleInfo(id,
					sample_name = sampleinfo$name,
					sample_info = sampleinfo$group);
			} else {
				# already clr vector of sampleinfo object?
				# just do nothing at here
			}
		}
	}

	x <- removes_missing(x, sampleinfo, percent = missing);
	x <- preprocessing(x, scale = factor);
	x;
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