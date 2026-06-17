require(mzkit);

let data = read.xcms_peaks("Z:\test.csv");
let sampleinfo = read.csv("Z:\sample_info.csv", row.names = 1, check.names = FALSE);
let [enrichment, metabolites] = mummichog_anno(peaks = data, sampleinfo = sampleinfo, libtype = 1);

print(enrichment);
print(metabolites);

write.csv(enrichment, file = "Z:/test_kegg_pathway.csv");
write.csv(metabolites, file = "Z:/test_anno.csv");