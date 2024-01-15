require(mzkit);
require(JSON);

imports "NLP" from "MLkit";
imports "SingleCells" from "mzkit";

let source = ?"--source" || stop();
let rawfiles = list.files(source, pattern = "*.mzPack");

let tree = SingleCells::cell_embedding(ndims = 20);

for(file in rawfiles) {
    let rawdata = file 
    |> open.mzpack(verbose = FALSE)
    ;
    tree |> embedding_sample(rawdata, tag = basename(file))
    ;

    print(basename(file));
}

let matrix = spot_vector(tree);

matrix 
|> as.data.frame()
|> write.csv(file = `${dirname(source)}/embedding.csv`)
;

tree 
|> cell_clusters
|> JSON::json_encode()
|> writeLines(
    con = `${dirname(source)}/clusters.json`
)
;