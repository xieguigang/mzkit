imports "mzkit.mrm" from "mzkit.quantitative.dll";

let mrmInfo as string = "T:\_ref\MetaCardio_STD_v5.xlsx";
let ions = read.ion_pairs(mrmInfo, "ion pairs");
let wiff as string = "T:\test\Data20190522liaoning-Cal";

wiff.standard_curve(wiff, ions,peakAreaMethod= 0, TPAFactors = NULL, calibrationNamedPattern = ".+[-]CAL[-]\d+", levelPattern = "[-]CAL[-]\d+") 
:> write.csv(file = "T:\test\L.csv");
