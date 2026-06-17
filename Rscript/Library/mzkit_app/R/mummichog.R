const mummichog_anno = function(peaks, sampleinfo, libtype = [1,-1], kegg = list(
    compounds = GCModeller::kegg_compounds(rawList = TRUE, reference_set = FALSE),
    pathways = GCModeller::kegg_maps()
)) {
    imports "Mummichog" from "mzkit";
    imports "sampleInfo" from "phenotype_kit";

    if (is.data.frame(sampleinfo)) {
        let sample_id = {
            if ("ID" in sampleinfo) {
                sampleinfo$ID;
            } else {
                rownames(sampleinfo);
            }
        };

        sampleinfo <- sampleInfo(ID = sample_id, sample_info = sampleinfo$sample_info,
                                sample_name = sampleinfo$sample_name);
    }

    let args = new("mummichog_pars", Mode = libtype);
    let mummichog = kegg_background(metabolites = kegg$compounds, pathways = kegg$pathways, 
        params=args);
    let [enrichment, metabolites] = mummichog |> peakList_annotation(peaks = peaks,
                                    sampleinfo = sampleinfo);

    list(enrichment = as.data.frame(enrichment), 
        metabolites = as.data.frame(metabolites));    
}