require(JSON);

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