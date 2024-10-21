require(mzkit);

imports "DIA" from "mzDIA";

setwd(@dir);

let testdata = read.csv("./test_ms2.csv", row.names = NULL, check.names = FALSE);

print(testdata, max.print= 6);

let spec = mzweb::parse_base64( mz = testdata$mz,
                                  intensity= testdata$intensity,
                                  id = testdata$splash_id);

print(spec);

let decompose = DIA::dia_nmf(spec, n = 5);
