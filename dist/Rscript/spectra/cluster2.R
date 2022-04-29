require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";
imports "visual" from "mzplot";

data = read.xlsx("D:/lipids_20220427.xlsx", row.names = 1);

print("view target lipids metabolite:");
print(data);

data = as.list(data, byrow = TRUE);
bin = spectrum.compares( equals_score = 0.95,
                        gt_score = 0.8
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

    rep = ions
	|> unionPeaks( matrix = TRUE)
	|> peakAnnotations(massDiff = 0.3)
	;
    summary = as.data.frame(ions);
    summary[, "scan"] = make.ROI_names(ions);
    summary[, "precursor_type"] = NULL;
    summary[, "collisionEnergy"] = NULL;
    summary[, "activation"] = NULL;
    summary[, "scan"] = NULL;
    summary[, "file"] = NULL;

	ref = strsplit(summary[, "lib_guid"],"#", fixed = TRUE);
	filename = sapply(ref, x -> x[1]);
	guid = sapply(ref, x -> x[2]);
	
	top = ions[which.max(summary[, "intensity"])];
	
	summary[, "lib_guid"] = guid;
	summary[, "filename"] = filename;

    print(summary);
    str(meta);    

    bitmap(file = `E:\lipids/${normalizeFileName(tag)}/union_rep.png`) {
        plot(rep, title = `${tag} [${meta$MainIon}]+`, size = [1600,900]);
    }

	bitmap(file = `E:\lipids/${normalizeFileName(tag)}/topIon.png`) {
        plot(top, title = `${tag} [${meta$MainIon}]+`, size = [1600,900]);
    }

    write.csv(summary , file = `E:\lipids/${normalizeFileName(tag)}/all_ions.csv`, row.names = FALSE);
}