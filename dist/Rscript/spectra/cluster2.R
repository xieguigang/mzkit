require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";

data = read.xlsx("D:/lipids_20220427.xlsx", row.names = 1);

print("view target lipids metabolite:");
print(data);

data = as.list(data, byrow = TRUE);
bin = spectrum.compares();

for(tag in names(data)) {
    file = `E:\lipids/${normalizeFileName(tag)}.mzPack`;
    ions = file 
    |> open.mzpack()
    |> ms2_peaks()
    |> spectrum_tree.cluster(
        compares = bin
    )
    |> cluster.nodes()
    ;
    ions = ions[which.max(sapply(ions, i -> [i]::Length))];
    ions = [ions]::cluster;
}