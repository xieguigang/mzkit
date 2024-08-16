require(mzkit);

imports "formula" from "mzkit";

setwd(@dir);

let demo_spectra = JSON::json_decode(readText("demo.json"));
let mz = as.numeric(demo_spectra@x);
let intensity = as.numeric(demo_spectra@y);

demo_spectra = libraryMatrix(data.frame(mz, intensity), parentMz = 720.0112);

print(demo_spectra);
str(demo_spectra |> formula::peakAnnotations(
    formula = "C15H25N5O20P4",
    adducts = ["[M+H]+"]
));