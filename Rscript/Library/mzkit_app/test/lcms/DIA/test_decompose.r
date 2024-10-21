require(mzkit);

imports "DIA" from "mzDIA";
imports "visual" from "mzplot";

setwd(@dir);

let testdata = read.csv("./test_ms2.csv", row.names = NULL, check.names = FALSE);

testdata = testdata[1:200,];

print(testdata, max.print= 6);

let spec = mzweb::parse_base64( mz = testdata$mz,
                                  intensity= testdata$intensity,
                                  id = testdata$splash_id);

print(spec);

let decompose = DIA::dia_nmf(spec, n = 5);
let sum = attr(decompose, "sum_spectrum");

for(name in names(sum)) {
    svg(file = `${name}.svg`) {
        plot(sum[[name]]);
    }
}