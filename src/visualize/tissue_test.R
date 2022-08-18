require(mzkit);
require(graphics2D);
require(filter);

imports "tissue" from "mzplot";

grid = `${@dir}/WeChat Image_20220818211220.jpg`
|> readImage()
|> tissue::scan_tissue(colors = "#f0b20f")
;

bitmap(file = `${@dir}/density.png`, size = [3060,4082], fill = "black");

grid 
|> heatmap_layer(target = "#f0b20f")
|> rasterHeatmap(gauss = 1, fillRect = TRUE)
;

dev.off();