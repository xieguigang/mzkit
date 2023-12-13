require(mzkit);

imports "chromatogram" from "mzkit";
imports "visual" from "mzplot";

let rawdata = open.mzpack("E:\\01183.mzpack");
let msdata = [rawdata]::MS;

str(as.list(msdata[1]));

let rt = [msdata]::rt;
let TIC = [msdata]::TIC;

print(max(rt));

let rowdata = data.frame(rt, into = TIC);
rowdata = rowdata[order(rt), ];
rowdata = rowdata[rowdata$rt between [max(rt) * 0.35, max(rt) *0.65], ];
rowdata[, "relative"] = rowdata$into / max(rowdata$into);

print(rowdata, max.print = 6);

write.csv(rowdata, file = `${@dir}/data_TIC.csv`);

rowdata = as.chromatogram(rowdata$rt, rowdata$into);

bitmap(file = `${@dir}/plot_TIC.png`) {
    plot(rowdata);
}

