require(clustering);
require(mzkit);
require(JSON);

imports "tissue" from "mzplot";

let image_raster = readImage("D:\\demo_test.PNG");
let nucleus_xy = mark_nucleus(image_raster, nucleus = ["#8230b8" "#903ab5" "#8826b1" "#9026ae" "#c23ec5"], 
    tolerance = 16);

let maps = scan_tissue(tissue =image_raster, colors = "#8230b8",
                               grid.size = 10,
                               tolerance = 15,
                               density.grid = 5);

write.csv(as.data.frame(maps), file = `${@dir}/scan_tissue.csv`, row.names = TRUE);

maps
|> JSON::json_encode()
|> writeLines(con = `${@dir}/scan_tissue.json`)
;

print(nucleus_xy, max.print = 6);

write.csv(nucleus_xy, file = `${@dir}/raster_nucleus.csv`, row.names = FALSE);

bitmap(file = `${@dir}/raw_raster.png`) {
    plot(nucleus_xy$x, nucleus_xy$y, reverse = TRUE, grid.fill = "white", point.size = 5);
}

rownames(nucleus_xy) = `${nucleus_xy$x},${nucleus_xy$y}`;

nucleus_xy = dbscan(nucleus_xy, eps = 1.25, minPts = 3);
nucleus_xy = as.data.frame([nucleus_xy]::cluster);

print(nucleus_xy, max.print = 6);

# str(nucleus_xy);

write.csv(nucleus_xy, file = `${@dir}/mark_nucleus.csv`, row.names = TRUE);

bitmap(file = `${@dir}/mark_nucleus.png`, padding = [100, 2400, 200, 200], size = [4800, 2100]) {
    plot(as.numeric(nucleus_xy$x), as.numeric(nucleus_xy$y), class= nucleus_xy$Cluster, 
        reverse = TRUE, grid.fill = "white",  colorSet = "paper", point.size = 5, 
        colorset.shuffles = TRUE,
        legend.block = 30);
}