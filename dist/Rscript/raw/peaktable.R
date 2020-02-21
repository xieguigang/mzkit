imports ["mzkit.assembly", "mzkit.math"] from "mzkit.dll";

setwd(!script$dir);

let raw as string = "../../../DATA/test/003_Ex2_Orbitrap_CID.mzXML";

ms1.scans(raw)
:> mz.deco()
:> as.data.frame
:> write.csv(file = `./${basename(raw)}.csv`);