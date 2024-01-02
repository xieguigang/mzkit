require(mzkit);

imports "mzDeco" from "mz_quantify";

let xic = list.files("G:\\tmp\\QC_pos", pattern = "*.xic");
# xic = lapply(xic, path -> readBin(path, what = "mz_group"), names = basename(xic));
let mzdata = read.csv("G:\\tmp\\QC_pos\\mzbins.csv", row.names = NULL);

xic = xic_pool(xic);

print(mzdata, max.print = 6);
# print(names(xic));

let peaksdata = xic 
|> mz_deco(tolerance = "da:0.01",
                            baseline = 0.65,
                            peak.width = [3,90],
                            joint = FALSE,
                            parallel = TRUE,                            
                            feature = mzdata$mz)
;

write.csv(peaksdata,
    file = "G:\\tmp\\QC_pos\\peaktable.csv"
);

write.csv(attr(peaksdata, "rt.shift"), file = "G:\\tmp\\QC_pos\\rt_shifts.csv");