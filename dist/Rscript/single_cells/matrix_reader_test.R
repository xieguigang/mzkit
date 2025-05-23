require(mzkit);

imports ["SingleCells", "MSI"] from "mzkit";

setwd(@dir);

const m = open.matrix("./brain1.dat");
const load = read.mz_matrix(m$reader);
const df = df.mz_matrix(load);

print(names(df));

print(rownames(df));

print(df[, 267.1339000012378]);


print(df["12,4", ]);