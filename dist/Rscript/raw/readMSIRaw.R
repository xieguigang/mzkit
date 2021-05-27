library("mzkit/ThermoRaw");

const raw = open.raw("F:\MSI\MPIBremen_Bputeoserpentis_MALDI-FISH_DHB_233x233pixel_3um_mz400-1200_240k@200.RAW");

for(i in 1:100) {
  const scan =  raw :> read.rawscan(i);
  
  print(scan :> events);
  print(scan :> logs);
  
  pause();
}