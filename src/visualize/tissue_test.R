require(mzkit);
require(graphics2D);

imports "tissue" from "mzplot";

grid = `${@dir}/WeChat Image_20220818211220.jpg`
|> readImage()
|> tissue::scan_tissue(colors = "#f0b20f")
;

bitmap(file = `${@dir}/density.png`, size = [2500,5000]);

grid 
|> heatmap_layer(target = "#f0b20f")
|> rasterHeatmap()
;

dev.off();