imports ["mzkit.assembly"] from "mzkit.dll";
imports ["mzkit.simulator"] from "mzkit.insilicons.dll";
imports ["mzkit.quantify.visual"] from "mzkit.quantify.dll";
# load debug api
imports "plot.charts" from "R.plot.dll";

setwd(!script$dir);

let mol <- ["C00008.txt"]
:> read.kcf
:> molecular.graph(verbose = TRUE);

let energyMax = (mol :> energy.range :> as.object)$Max;

print("Max energy of current molecular bounds:");
print(energyMax);

let mu = 60;
let sd = 10;

let lib <- mol
:> fragmentation(energy = energy.normal(mu, sd, energyMax),nIntervals=100)
:> centroid
;

plot(x -> (1/(sd * sqrt(2* PI))) * exp(-( ( x-mu )^2 )/( 2*(sd ^ 2) ))   , x= 0: energyMax step 1)
:> save.graphics(file = "energy.png")
;

lib :> write.mgf(file = "ADP.mgf");

lib 
:> mass_spectrum.plot 
:> save.graphics(file = "ADP.png");