imports "mzkit.quantify.visual" from "mzkit.quantify.dll";

setwd(!script$dir);

let mz as double = [53.456,89.123,156.48564,178.646451,231.23];
let into as double = [0.03,0.9,1.0,0.64,0.56];

data.frame(mz = mz, into = into)
:> mass_spectrum.plot()
:> save.graphics(file = "./demo_spectrum.png")
;


# plot from dataframe input

read.csv("mz_centroid.csv")
:> mass_spectrum.plot()
:> save.graphics(file = "./HCD_centroid_bymzkit.png")
;