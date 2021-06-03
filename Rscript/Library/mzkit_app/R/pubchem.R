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
    const section_data  = getQuery("section.graphquery");
    const json = document
    |> graphquery::query(pugView_query)
    ;

    str(lapply(json$Data[[3]]$rawHtml, function(html) {
        Html::parse(html)
        |> graphquery::query(section_data)
        ;
    }));

    NULL;
}

const getQuery as function(fileName) {
    `graphquery/${fileName}`
    |> system.file(package = "mzkit")
    |> readText
    |> graphquery::parseQuery
    ;
}