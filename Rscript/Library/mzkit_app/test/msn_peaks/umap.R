require(umap);

imports "clustering" from "MLkit";

let data = read.csv(relative_work("msn.csv"), row.names = 1, check.names = FALSE);

str(data);

let scatter = umap(data, numberOfNeighbors = 128, dimension = 9);

scatter = as.data.frame(scatter$umap, labels = scatter$labels);
scatter = kmeans(scatter, centers = 9,
                                bisecting = TRUE);

scatter = as.data.frame(scatter);

str(scatter);

bitmap(file = relative_work("umap.png")) {
    plot(as.numeric(scatter$dimension_1), as.numeric( scatter$dimension_2), fill = "white", colors = "paper", class = scatter$Cluster);
}

write.csv(scatter, file = relative_work("umap.csv"));

scatter = scatter |> groupBy("Cluster");

for(let cluster in scatter) {
    # print(cluster);
    print(as.numeric(rownames(cluster)));
}

scatter = data.frame(cluster = names(scatter), size = sapply(scatter, part -> nrow(part)));

print(scatter);

