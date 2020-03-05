imports "mzkit.mrm" from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

let ions = ["N:\project\HT2019120218003 深圳微生太科技有限公司 江舜尧\原始数据\MRM\targets-bileacid.MSL"]
:> read.msl(unit = "Minute")
:> as.ion_pairs
;

["N:\project\HT2019120218003 深圳微生太科技有限公司 江舜尧\原始数据\MRM\cal"]
:> list.files(pattern = "*.mzML")
:> MRM.rt_alignments(ions = ions)
;