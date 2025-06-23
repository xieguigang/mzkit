const MRM_dataReport = function(xic, tpa) {
    require(Markdown2PDF);
    require(JSON);

    let local_res = system.file("data/MRM/peaksdata.html", package = "mzkit");
    let template = htmlReport::htmlTemplate(local_res);

    template = template + list(
        xic = base64_encode(JSON::json_encode(xic)),
        area = base64_encode(JSON::json_encode(tpa))
    );

    .Internal::html(template);
}