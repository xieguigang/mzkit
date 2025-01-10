require(mzkit);

imports "math" from "mzkit";
imports "formula" from "mzkit";

let gallic_acid =  "C7H6O5";

print(math::mz(formula::eval(gallic_acid), "[2M-H2O-H]-"));
