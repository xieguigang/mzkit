require(mzkit);

imports ["formula", "math", "data"] from "mzkit";
imports "visual" from "mzplot";

setwd(@dir);

input = read.csv("./LSM(d16-1)+H.csv", row.names = NULL);
annos = read.csv("lipids.csv", row.names = NULL);

print(annos);
formula::registerAnnotations(annos);

print(input);

bitmap(file = "./LSM(d16-1)+H.png") {

input[, "annotation"] = NULL;
ms2 = input
|> libraryMatrix(, parentMz = 437.3153)
|> peakAnnotations(massDiff = 0.1)
;

print(as.data.frame(ms2));

plot(ms2, title = "LSM(d16:1)+H", label.intensity = 0)
;

}
