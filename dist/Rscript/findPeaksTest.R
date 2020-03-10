imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";

let name as string = "NorDCA";
let data = [`S:\胆汁酸保留时间漂移测试\20200309三条标曲\test\cal.chromatogramPlots\20200305-cal10\chromatogramROI/${name}.csv`]
:> read.csv
:> as.chromatogram("Time", "Intensity")
:> peakROI
:> as.data.frame
:> print
;