imports "mzkit.mrm" from "mzkit.quantitative.dll";

let mzML as string = "D:\biodeep\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\data\day3_test\files\day3-L44.mzML";
let mrmInfo as string = "T:\_ref\MetaCardio_STD_v5.xlsx";
let ions = read.ion_pairs(mrmInfo, "ion pairs");