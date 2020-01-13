imports "mzkit.assembly" from "mzkit.dll";
imports "mzkit.quantify.visual" from "mzkit.quantify.dll";

["../../DATA\test\HCD_profiles.txt"]
:> read.mgf()[1]
:> mass_spectrum.plot
:> save.graphics(file = "./HCD_profiles.png")
;