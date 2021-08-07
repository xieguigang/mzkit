imports ["mzweb", "MSI"] from "mzkit";

"E:\demo\HR2MSI mouse urinary bladder S096.mzPack"
|> open.mzpack
|> peakMatrix
|> write.csv(file = "E:\demo\HR2MSI mouse urinary bladder S096_top3.csv")
;