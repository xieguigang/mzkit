imports "mzkit.quantify.visual" from "mzkit.quantify.dll";

let mz as double = [56.456,89.123,156.48564,178.646451,231.23];
let into as double = [1545623,12319999,231564312,3123134564,564156456];

data.frame(mz = mz, into = into)
:> mass_spectrum.plot()
:> save.graphics(file = "./demo_spectrum.png")