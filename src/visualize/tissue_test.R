require(mzkit);

imports "tissue" from "mzplot";

grid = `${@dir}/WeChat Image_20220818211220.jpg`
|> readImage()
|> tissue::scan_tissue()
;