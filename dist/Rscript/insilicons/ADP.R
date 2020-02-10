imports ["mzkit.assembly"] from "mzkit.dll";
imports ["mzkit.simulator"] from "mzkit.insilicons.dll";
imports ["mzkit.quantify.visual"] from "mzkit.quantify.dll";

setwd(!script$dir);

let mol <- ["C00008.txt"]
:> read.kcf
:> molecular.graph(verbose = TRUE);

let energyMax = (mol :> energy.range :> as.object)$Max;

print("Max energy of current molecular bounds:");
print(energyMax);

let lib <- mol
:> fragmentation(energy = energy.normal(100, 5, energyMax),nIntervals=100)
:> centroid
;

lib :> write.mgf(file = "ADP.mgf");

lib 
:> mass_spectrum.plot 
:> save.graphics(file = "ADP.png");