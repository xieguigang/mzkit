imports "NMR" from "mzkit";

"D:\mzkit\DATA\nmr\FAM013_AHTM.PROTON_04.nmrML"
|> NMR::read.nmrML
|> acquisition
|> FID
|> print
;