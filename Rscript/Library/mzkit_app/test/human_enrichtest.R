require(GCModeller);
require(mzkit);

imports ["background", "GSEA"] from "gseakit";


cid = read.csv("D:\mzkit\Rscript\demo\Mummichog\mz_annotations.csv")$hits;
cid = unlist(strsplit(cid, "[;]\s+"));
cid = unlist(strsplit(cid, "\s+"));
cid = unique(cid[cid == $"C\d+"]);

print(cid);


data = system.file("data/hsa_metpa.json", package = "mzkit");

print(data);

data = data |> readText() |> json_decode(typeof = "metpa");

print("view of the loaded background model:");
str(data);

enrich = metpa_enrich(cid, data);

print(enrich, max.print = 13);
