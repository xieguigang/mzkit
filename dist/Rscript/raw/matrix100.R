imports ["mzweb", "MSI"] from "mzkit";

"E:\demo\HR2MSI mouse urinary bladder S096.mzPack"
|> open.mzpack
|> peakSamples(resolution = 75, mzError = "ppm:20", cutoff = 0.01)
|> write.csv(file = "E:\demo\HR2MSI mouse urinary bladder S096_top3.csv")
;