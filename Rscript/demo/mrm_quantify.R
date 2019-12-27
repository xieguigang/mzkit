imports "mzkit.mrm" from "mzkit.quantitative.dll";

let mrmInfo as string = "T:\_ref\MetaCardio_STD_v5.xlsx";
let ions = read.ion_pairs(mrmInfo, "ion pairs");
let wiff as string = "T:\test\Data20190522liaoning-Cal";

list.files(wiff, pattern = "*.mzML")
:> wiff.scans(ions,peakAreaMethod= 0, TPAFactors = NULL, ) 
:> write.csv(file = "T:\test\L.csv");

wiff = "T:\test\Data20190222";

list.files(wiff, pattern = "*.mzML")
:> wiff.scans(ions,peakAreaMethod= 0, TPAFactors = NULL, ) 
:> write.csv(file = "T:\test\samples.csv");