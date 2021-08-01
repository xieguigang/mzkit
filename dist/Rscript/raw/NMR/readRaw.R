imports "NMR" from "mzkit";

# "E:\\mzkit\\DATA\\nmr\\HMDB00005.nmrML"
# |> NMR::read.nmrML
# |> acquisition
# |> FID
# |> print
# ;

"E:\\mzkit\\DATA\\nmr\\HMDB00005.nmrML"
|> NMR::read.nmrML
|> spectrumList
|> spectrum(NMR::read.nmrML("E:\\mzkit\\DATA\\nmr\\HMDB00005.nmrML"))
|> print
;