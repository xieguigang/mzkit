imports "mzkit.mrm" from "mzkit.quantify.dll";

let mzML as string = "D:\biodeep\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\data\day3_test\files\day3-L44.mzML";
let mrmInfo as string = "T:\_ref\MetaCardio_STD_v5.xlsx";
let ions = read.ion_pairs(mrmInfo, "ion pairs");

let mrm.ions = extract.ions(mzML, ions);

# for(ion in mrm.ions) {
	# print(ion :> as.object :> do.call(calls = "name"));
# }