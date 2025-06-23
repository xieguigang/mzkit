const MRM_dataReport = function(xic, tpa) {
    require(Markdown2PDF);
    require(JSON);

    let local_res = system.file("data/MRM/peaksdata.html", package = "mzkit");
    let template = htmlReport::htmlTemplate(local_res);
    let peaktable = as.data.frame(unlist(tpa), peaktable = TRUE);
    let pca = prcomp(peaktable, pc = 2);

    template = template + list(
        xic = base64(JSON::json_encode(xic)),
        area = base64(JSON::json_encode(tpa)),
        pca = base64(JSON::json_encode(pca$score))
    );

    .Internal::html(template);
}