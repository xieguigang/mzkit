require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb", "data"] from "mzkit";

data = read.xlsx("D:/lipids_20220427.xlsx", row.names = 1);
ions = list();

print("view target lipids metabolite:");
print(data);

data = as.list(data, byrow = TRUE);

str(data);

for(rawfile in open.mzwork("E:/lipids.mzWork")) {
    tag = [rawfile]::source;
    ions[[tag]] = lapply(data, function(i) {
        mz = i$mz;
        rt = i$rtmin * 60;

        rawfile 
        |> ms2_peaks(mz)
        |> rt_slice(rtmin = rt - 15, rtmax = rt + 15)
        ;
    });

    NULL;
}

print("union ions...");

lipidIons = list();

for(file in ions) {
    for(name in names(file)) {
        lipidIons[[name]] = append(lipidIons[[name]], file[[name]]); 
    }

    cat(".");
}

cat("\n");
print("save ions to mzpack...");

for(lipid in names(lipidIons)) {
    unlist(lipidIons[[lipid]])
    |> as.vector()
    |> packData()
    |> write.mzPack(file = `E:/lipids/${normalizeFileName(lipid)}.mzPack`)
    ;

    print(lipid);
}

print("~job done!");