require(mzkit);

imports "formula" from "mzkit";

const H2O = formula::scan("H2O");
const D_glucose = formula::scan("C6H12O6"); 

print(H2O);
print(D_glucose);

const diglucose = D_glucose * 2 - H2O;

cat("\n\n");

print("diglucose:");
print(diglucose);

print("the compose formula:");
print(toString(diglucose));

const rhamnoside = formula::scan("C6H12O5");

