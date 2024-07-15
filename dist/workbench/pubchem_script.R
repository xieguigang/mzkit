require(mzkit);

imports "pubchem_kit" from "mzkit";

let refmet = read.csv(file.path(@dir, "refmet.csv"), row.names = 1, check.names = FALSE);
let save_repo = file.path(@dir, "repo");

print(refmet, max.print = 6);

pugView(402,cacheFolder = save_repo);

for(let m in tqdm(as.list(refmet, byrow = TRUE))) {
    if (m$pubchem_cid > 0) {
        pugView(m$pubchem_cid,cacheFolder = save_repo, sleep = 3);
        # sleep(3);
    }

    NULL;
}