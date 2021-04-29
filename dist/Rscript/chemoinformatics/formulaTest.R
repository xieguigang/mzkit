require(mzkit);

imports "formula" from "mzkit";

const H2O = formula::scan("H2O");
const D_glucose = formula::scan("C6H12O6"); 

const diglucose = D_glucose * 2 - H2O;

print(diglucose);