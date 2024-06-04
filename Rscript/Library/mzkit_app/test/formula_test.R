require(mzkit);

imports "formula" from "mzkit";

let demo  = C9H6O3KNaNH5;

print(demo);
print(formula_calibration(C9H6O3KNaNH5, ["[M+H]+", "[M+K]+","[M+H-H2O]+","[M+NH3]-","[M]+","[M]-","[M-H]-","[M+COO]-","[M-H2O]-"]));