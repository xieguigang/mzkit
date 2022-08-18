require(mzkit);
require(graphics2D);

imports "tissue" from "mzplot";

grid = `${@dir}/WeChat Image_20220818211220.jpg`
|> readImage()
|> tissue::scan_tissue()
;

bitmap(file = `${@dir}/density.png`, size = [600,600]);

grid 
|> heatmap_layer()
|> rasterHeatmap()
;

dev.off();