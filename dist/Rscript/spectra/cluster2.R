require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";
imports "visual" from "mzplot";

data = read.xlsx("D:/lipids_20220427.xlsx", row.names = 1);

print("view target lipids metabolite:");
print(data);

data = as.list(data, byrow = TRUE);
bin = spectrum.compares( equals_score = 0.9,
                        gt_score = 0.65
                        );

for(tag in names(data)) {
    file = `E:\lipids/${normalizeFileName(tag)}.mzPack`;
    meta = data[[tag]];
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
    summary[, "scan"] = make.ROI_names(ions);
    summary[, "precursor_type"] = NULL;
    summary[, "collisionEnergy"] = NULL;
    summary[, "activation"] = NULL;
    summary[, "scan"] = NULL;
    summary[, "file"] = NULL;

    print(summary);
    str(meta);    

    bitmap(file = `E:\lipids/${normalizeFileName(tag)}/plot.png`) {
        plot(rep, title = `${tag} [${meta$MainIon}]+`, size = [1600,900]);
    }

    write.csv(summary , file = `E:\lipids/${normalizeFileName(tag)}/all_ions.csv`, row.names = FALSE);
}