imports ["mzkit.assembly"] from "mzkit.dll";
imports ["mzkit.simulator"] from "mzkit.insilicons.dll";

setwd(!script$dir);

let mol <- ["C00008.txt"]
:> read.kcf
:> molecular.graph(verbose = TRUE);

let energyMax = (mol :> energy.range :> as.object)$Max;

print("Max energy of current molecular bounds:");
print(energyMax);

mol
:> fragmentation(energy = energy.normal(10, 0.05, energyMax),nIntervals=100)
:> centroid
:> write.mgf(file = "ADP.mgf")
;