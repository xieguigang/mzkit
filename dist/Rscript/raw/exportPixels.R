library("mzkit/ThermoRaw");
library("mzkit/mzweb");

const rawmzpack as string = "F:\MSI\MPIBremen_Bputeoserpentis_MALDI-FISH_DHB_233x233pixel_3um_mz400-1200_240k@200.mzpack";

write.csv(file = `${dirname(rawmzpack)}/${basename(rawmzpack)}_pixels.csv`) {

rawmzpack
|> open.mzpack
|> MSI_pixels
;

}
