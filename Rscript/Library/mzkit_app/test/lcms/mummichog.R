require(mzkit);

let data = read.xcms_peaks("Z:\test.csv");
let sampleinfo = read.csv("Z:\sample_info.csv", row.names = 1, check.names = FALSE);
let [enrichment, metabolites] = mummichog_anno(peaks = data, sampleinfo = sampleinfo, libtype = 1);

print(enrichment);
print(metabolites);

