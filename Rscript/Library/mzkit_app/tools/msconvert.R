require(mzkit);
require(xlsx);

imports ["mzPack", "mzweb"] from "mzkit";

let rawdata_dir = ?"--input" || stop("A data directory for rawdata input should not be empty!");
let output_dir = ?"--outputdir" || rawdata_dir;
let rawfiles = list.files(rawdata_dir, pattern = "*.raw");

print(basename(rawfiles));

for(let file in tqdm(rawfiles)) {
    let rawdata = open.mzpack(file);
    let outfile = file.path(output_dir, "PACK", `${basename(file)}.mzPack`);
    let outxml = file.path(output_dir,"XML", `${basename(file)}.mzXML`);

    rawdata |> write.mzPack(file = outfile);
    rawdata |> convertTo_mzXML(file = outxml);

    invisible(NULL);
}

cat("\n\n");
cat("Mass Spectrum Rawdata File Conversion Job Done!\n");