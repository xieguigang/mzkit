imports ["mzkit.assembly", "mzkit.math"] from "mzkit.dll";

setwd(!script$dir);

let raw as string = "../../../DATA/test/003_Ex2_Orbitrap_CID.mzXML";
let mz <- ms1.scans(raw) :> mz.groups();

mz[1]
:> as.data.frame
:> write.csv(file = `./XIC_mz=${as.object(mz[1])$mz}.csv`);