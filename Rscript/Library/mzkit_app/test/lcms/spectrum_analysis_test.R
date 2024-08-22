require(mzkit);

imports "formula" from "mzkit";

let speca = data.frame(
    mz = [57.0701
149.0236
150.0268
255.8823
279.0946],
    intensity = [
        8795.362305
        120855.2109
        10062.88086
        14960.03906
        20618.51172
    ]
);
let formula = "C18H30O2";
let adduct = "[M+H]+";

speca = libraryMatrix(speca, parentMz = 278.224568 + 1,centroid = TRUE);
speca = formula::peaks_annotation(speca, formula, adducts = adduct, massDiff = 0.3, as.list = TRUE,
    unset.scalar = TRUE );

str(speca);

print(as.data.frame(speca$products));
