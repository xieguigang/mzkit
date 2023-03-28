require(GCModeller);
require(mzkit);

data = system.file("data/hsa_metpa.json", package = "mzkit");
data = data |> readText() |> json_decode(typeof = "");