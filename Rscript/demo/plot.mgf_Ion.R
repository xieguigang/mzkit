imports "mzkit.assembly" from "mzkit.dll";
imports "mzkit.quantify.visual" from "mzkit.quantify.dll";

setwd(!script$dir);

let ions <- ["../../DATA\test\HCD_profiles.txt"]
:> read.mgf()
;

ions[1]
:> mass_spectrum.plot
:> save.graphics(file = "./HCD_profiles.png")
;

ions
:> mgf.ion_peaks
:> centroid
:> first
:> mass_spectrum.plot
:> save.graphics(file = "./HCD_centroided.png")
;