imports "MSI" from "mzkit";
imports "MsImaging" from "mzkit.plot";

const raw   = MSI::open.imzML("D:\mzkit\DATA\test\imzML\S042_Continuous_imzML1.1.1\S042_Continuous.imzML");
const cache = write.MSI_XIC(
	pixels = raw$scans,
	ibd    = raw$ibd
); 

write.MSI(cache, file = "D:\mzkit\DATA\test\imzML\S042_Continuous_imzML1.1.1.MSI");
