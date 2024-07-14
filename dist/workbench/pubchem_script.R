require(mzkit);

imports "pubchem_kit" from "mzkit";

let refmet = read.csv("G:\Daisy\build\refmet.csv", row.names = 1, check.names = FALSE);

print(refmet, max.print = 6);

for(let m in tqdm(as.list(refmet, byrow = TRUE))) {
    pugView(m$pubchem_cid,cacheFolder = "Z:\test");
}