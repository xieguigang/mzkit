require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";
imports "visual" from "mzplot";

ions = list();
mz = 302.304543;
bin = spectrum.compares( equals_score = 0.9,
                        gt_score = 0.65
                        );
						
for(rawfile in open.mzwork("E:/lipids.mzWork")) {
    tag = [rawfile]::source;
    # ions[[tag]] = lapply(data, function(i) {
        # # mz = i$mz;
        # # rt = i$rtmin * 60;

        # rawfile 
        # |> ms2_peaks(mz)
        # # |> rt_slice(rtmin = rt - 15, rtmax = rt + 15)
        # ;
    # });

	ions[[tag]] = rawfile |> ms2_peaks(mz);

    NULL;
}

# print("union ions...");

# lipidIons = list();

# for(file in ions) {
    # for(name in names(file)) {
        # lipidIons[[name]] = append(lipidIons[[name]], file[[name]]); 
    # }

    # cat(".");
# }

# cat("\n");
# print("save ions to mzpack...");

# for(lipid in names(lipidIons)) {
    # unlist(lipidIons[[lipid]])
    # |> as.vector()
    # |> packData()
    # |> write.mzPack(file = `E:/lipids/${normalizeFileName(lipid)}.mzPack`)
    # ;

    # print(lipid);
# }

# print("~job done!");

ions = unlist(ions)
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

    bitmap(file = `E:\lipids/So[d18_0]_plot.png`) {
        plot(rep, title = "So(d18:0) [M+H]+", size = [1600,900]);
    }

    write.csv(summary , file = `E:\lipids/So[d18_0]_ions.csv`, row.names = FALSE);

# write.csv(data, file = "E:/lipids_SO.csv", row.names = FALSE);