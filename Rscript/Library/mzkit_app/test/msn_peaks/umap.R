require(umap);

imports "clustering" from "MLkit";

let data = read.csv(relative_work("msn_norm.csv"), row.names = 1, check.names = FALSE);
let rsd_val  =apply(data, margin = 1, FUN = rsd);

str(data);
print(rsd_val);

let scatter = umap(data, numberOfNeighbors = 128, dimension = 9);

scatter = as.data.frame(scatter$umap, labels = scatter$labels);
scatter = kmeans(scatter, centers = 9,
                                bisecting = TRUE);

scatter = as.data.frame(scatter);
scatter[,"rsd"] = rsd_val;

str(scatter);

bitmap(file = relative_work("umap.png")) {
    plot(as.numeric(scatter$dimension_1), as.numeric( scatter$dimension_2), fill = "white", colors = "paper", class = scatter$Cluster);
}

write.csv(scatter, file = relative_work("umap.csv"));

scatter = scatter |> groupBy("Cluster");

let i = 0;

for(let cluster in scatter) {
    cluster = cluster[order(cluster$rsd, decreasing=TRUE),];
    cluster = cluster[1:20,];

    print(as.numeric(rownames(cluster)));

    write.csv( data.frame(mz =  rownames(cluster), w = cluster$rsd), file = relative_work(`class_${i=i+1}.csv`), row.names = FALSE);
}

scatter = data.frame(cluster = names(scatter), size = sapply(scatter, part -> nrow(part)));

print(scatter);

