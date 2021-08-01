imports "NMR" from "mzkit";

"E:\\mzkit\\DATA\\nmr\\HMDB00005.nmrML"
|> NMR::read.nmrML
|> acquisition
|> FID
|> print
;