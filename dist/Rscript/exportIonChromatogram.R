imports "mzkit.mrm" from "mzkit.quantify.dll";
imports "mzkit.assembly" from "mzkit.dll";

let name as string = "NorDCA";

let ion <- ["S:\胆汁酸保留时间漂移测试\20200309三条标曲\targets-bileacid.MSL"]
:> read.msl(unit = "Minute") 
:> as.ion_pairs 
:> which(ion -> as.object(ion)$name == name) 
:> first
;

ion <- ["S:\胆汁酸保留时间漂移测试\20200309三条标曲\test\cal\20200305-cal10.mzML"]
:> extract.ions(ion)
:> first
:> as.object
;

let save.dir as string = "S:\胆汁酸保留时间漂移测试\20200309三条标曲\test\cal.chromatogramPlots\20200305-cal10\chromatogramROI";

ion$chromatogram 
:> write.csv(file = `${save.dir}\${name}.csv`)
;