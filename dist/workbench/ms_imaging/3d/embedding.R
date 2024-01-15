require(mzkit);

imports "SingleCells" from "mzkit";

let source = ?"--source" || stop();
let rawfiles = list.files(source, pattern = "*.mzPack");

let tree = SingleCells::cell_embedding();

for(file in rawfiles) {
    let rawdata = file 
    |> open.mzpack()
    ;
    tree |> embedding_sample(rawdata)
    ;
}

let matrix = spot_vector(tree);