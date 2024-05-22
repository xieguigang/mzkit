require(mzkit);

let data = open.mzpack("F:\\0_A1(1).mzML");
let ms1 = centroid( ms1_scans(data),  tolerance = "da:0.001",
                              intoCutoff = 0);

as.data.frame(ms1)
|> write.csv(file = "F:\\0_A1(1)_ms1.csv")
;