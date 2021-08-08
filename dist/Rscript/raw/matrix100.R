imports ["mzweb", "MSI"] from "mzkit";

"E:\demo\HR2MSI mouse urinary bladder S096.mzPack"
|> open.mzpack
|> peakSamples(resolution = 175, mzError = "da:0.05", cutoff = 0.005)
|> write.csv(file = "E:\demo\HR2MSI mouse urinary bladder S096_top3.csv")
;