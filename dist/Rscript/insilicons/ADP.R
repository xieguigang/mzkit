imports ["mzkit.simulator"] from "mzkit.insilicons.dll";

setwd(!script$dir);

["C00008.txt"]
:> read.kcf
:> molecular.graph(verbose = TRUE)
:> fragmentation(energy = energy.normal(10, 0.05))
:> write.mgf(file = "ADP.mgf")
;