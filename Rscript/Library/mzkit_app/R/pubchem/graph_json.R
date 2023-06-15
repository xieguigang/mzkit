require(JSON);

#' Parse the pugview xml as the metabolite data object
#'
#' @param dataXml the xml content of the pugview data object
#' @param process a callable function that used for additional object processing, example like
#'    attach a additional database cross reference id to the xrefs of the generated pugview 
#'    metabolite data object
#' @param extensionCache a cache dir for save the extension data of the current metabolite object,
#'    example as pathway query, reaction query, and reaction query.
#'
#' @return A generated metabolite object that could be used for dump well formatted json
#'
const pubchem_graphjson = function(dataXml, process = NULL, extensionCache = "./.cache/extdata/") {
    let pugView = as.list(metadata.pugView(read.pugView(dataXml)));
    let data = {
        if (!is.null(process)) {
            process(pugView);
        } else {
            pugView
        }
    }

	str(data);
	str(data$synonym);
	
	ext = [data$"ID"] 
    |> query.external(
		cache    = extensionCache, 
		interval = 0
	) 
    |> lapply(JSON::json_decode)
    ;

	data$pathways = ext$pathways;
	data$organism = ext$taxonomy;
	data$reaction = ext$reaction;
    data;
}