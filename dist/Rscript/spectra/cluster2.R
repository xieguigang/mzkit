require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";
imports "visual" from "mzplot";

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

    rep = unionPeaks(ions);
    summary = as.data.frame(ions);

    print(summary);

    bitmap(file = `E:\lipids/${normalizeFileName(tag)}/plot.png`) {
        plot(centroid(rep));
    }

    write.csv(summary , file = `E:\lipids/${normalizeFileName(tag)}/all_ions.csv`);
}