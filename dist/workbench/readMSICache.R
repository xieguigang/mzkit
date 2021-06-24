imports "MsImaging" from "mzkit.plot";

const cache = open.MSI("E:\mzkit\DATA\test\imzML\S042_Continuous_imzML1.1.1.MSI");
const layer = cache 
|> pixels(mz = [229]) 
|> as.vector
;

print(layer);

pause();