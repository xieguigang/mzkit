require(mzkit);

imports "DIA" from "mzDIA";
imports "visual" from "mzplot";
imports "MoleculeNetworking" from "mzDIA";

setwd(@dir);

let spectrum = read.cache("F:\testdata.spec");
let cluster = spectrum_grid(spectrum, dotcutoff = 0.9);
let i = 1;

for(let line in cluster |> spectrum_clusters()) {
    let decompose = line |> dia_nmf(n = 2);
    let sum = attr(decompose, "sum_spectrum");

    for(name in names(sum)) {
        svg(file = `group_${i}-LIPID_${name}.svg`) {
            plot(sum[[name]]);
        }
    }

    i = i + 1;
}
