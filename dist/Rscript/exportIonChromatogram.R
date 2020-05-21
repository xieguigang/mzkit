imports "mzkit.mrm" from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

let name as string = "GUCA";

let ion <- ["F:\MRM_test\project\HT2020051223004\GUCA.MSL"]
:> read.msl(unit = "Minute") 
:> as.ion_pairs 
:> which(ion -> as.object(ion)$name == name) 
:> first
;

ion <- ["F:\MRM_test\project\HT2020051223004\cal\cal1.mzML"]
:> extract.ions(ion)
:> first
:> as.object
;

let save.dir as string = "F:\MRM_test\project\HT2020051223004";

ion$chromatogram 
:> write.csv(file = `${save.dir}/${name}.csv`)
;