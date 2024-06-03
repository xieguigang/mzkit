require(mzkit);

imports "formula" from "mzkit";

print(formula_calibration(C9H6O3KNaNH5, ["[M+H]+", "[M+K]+","[M+H-H2O]+","[M+NH3]-","[M]+"]));