require(GCModeller);
require(mzkit);

imports ["background", "GSEA"] from "gseakit";

data = system.file("data/hsa_metpa.json", package = "mzkit");
data = data |> readText() |> json_decode(typeof = "metpa");

print("view of the loaded background model:");
str(data);

