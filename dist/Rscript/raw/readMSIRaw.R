library("mzkit/ThermoRaw");
library("mzkit/mzweb");

const rawfile = "F:\MSI\MPIBremen_Bputeoserpentis_MALDI-FISH_DHB_233x233pixel_3um_mz400-1200_240k@200.RAW";

using raw as open.raw(rawfile) {

# for(i in 1:100) {
  # const scan =  raw :> read.rawscan(i);
  
  # print(scan :> events);
  # print(scan :> logs);
  
  # pause();
# }

	raw 
	|> load_MSI(pixels = [233, 233])
	|> write.mzPack(file = `${dirname(rawfile)}/${basename(rawfile)}.mzpack`)
	;

}

