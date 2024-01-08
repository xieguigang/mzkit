require(mzkit);

imports "tissue" from "mzplot";

let image_raster = readImage("D:\\demo_test.PNG");
let nucleus_xy = mark_nucleus(image_raster);

print(nucleus_xy, max.print = 6);

write.csv(nucleus_xy, file = `${@dir}/raster_nucleus.csv`, row.names = FALSE);


