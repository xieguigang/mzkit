imports "pubchem_kit" from "mzkit";
imports "Html" from "webKit";
imports "graphquery" from "webKit";

require(REnv);

const pubchem_meta as function(term) {
    const cache = getOption("pubchem.http_cache") || stop("the 'pubchem.http_cache' location is not set!");
    const sleep = getOption("http.sleep") || 3;
    const cid   = pubchem_kit::CID(term, cache);

    options(http.cache_dir = cache);

    if (length(cid) == 0) {
        print(`no cid was found for metabolite term: '${term}'...`);
        NULL;
    } else {
        print(`found ${length(cid)} mapping to term: '${term}'!`);

        cid
        |> lapply(function(id) {
            id 
            |> pubchem_url 
            |> REnv::getHtml 
            |> Html::parse
            ;
        }, names = cid)
        |> lapply(parsePubchemMeta)
        ;
    } 
}

const parsePubchemMeta as function(document) {
    const pugView_query = getQuery("pubchem.graphquery");
    const section_data  = getQuery("section_data.graphquery");
    const json = document
    |> graphquery::query(pugView_query)
    ;
    const data  = lapply(json$Data, function(sec) sec$rawHtml, names = i -> i$name);
    const identifiers = data$"Names and Identifiers" 
    |> lapply(function(html) {
        Html::parse(html)
        |> graphquery::query(section_data)
        ;
    })
    |> lapply(x -> x, names = x -> x$name)
    ;
    const names       = parseNames(identifiers$"Synonyms");
    const formula     = getDataValues(identifiers$"Molecular Formula");
    const descriptors = parseDescriptors(identifiers$"Computed Descriptors"); 
    const xref        = parseXref(json$Reference);   

    # print(formula);
    # print(descriptors);
    # print(xref);

    # str(json);

    list(
        CID = json$CID
    );
}

const parseXref as function(refs) {
    refs = data.frame(
        id      = refs |> sapply(x -> x$id),
        dbName  = refs |> sapply(x -> x$database),
        xref    = refs |> sapply(x -> x$xref),
        synonym = refs |> sapply(x -> x$synonym),
        guid    = refs |> sapply(x -> x$guid),
        url     = refs |> sapply(x -> x$url)
    );
    refs = refs[order(refs[, "dbName"]), ];
    refs;
} 

const parseDescriptors as function(descriptors) {
    descriptors$dataList
    |> lapply(getDataValues, names = x -> x$name)
    ;
}

#' get values in section$data
#' 
const getDataValues as function(section) {
    sapply(section$data, x -> x$value);
}

const parseNames as function(names) {
    names = lapply(names$dataList, x -> x$data, names = x -> x$name);
    
    names$"Removed Synonyms" = NULL;
    names
    |> sapply(v -> sapply(v, x -> x$value))
    |> unlist
    |> unique
    |> orderBy(x -> nchar(x))
    ;
}

const getQuery as function(fileName) {
    `graphquery/${fileName}`
    |> system.file(package = "mzkit")
    |> readText
    |> graphquery::parseQuery
    ;
}

const mesh_model = function() {
    "data/mtrees2022.bin"
    |> system.file(package = "mzkit")
    |> read.mesh_tree()
    |> mesh_background()
    ;
}