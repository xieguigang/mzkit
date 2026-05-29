const MRM_dataReport = function(xic, tpa) {
    require(Markdown2PDF);
    require(JSON);

    imports "stats" from "Rlapack";

    let local_res = system.file("data/MRM/peaksdata.html", package = "mzkit");
    let template = htmlReport::htmlTemplate(local_res);
    let peaktable = as.data.frame(unlist(tpa), peaktable = TRUE);
    let pca = prcomp(peaktable, pc = 2);
    let loading = pca$loading;

    pca = pca$score;
    pca[,"sample"] = rownames(peaktable);
    loading[,"ion"] = colnames(peaktable);

    template = template + list(
        xic = base64(JSON::json_encode(xic)),
        area = base64(JSON::json_encode(tpa)),
        pca = base64(JSON::json_encode(pca)),
        loading = base64(JSON::json_encode(loading))
    );

    .Internal::html(template);
}