# require(mzkit);

imports "formula" from "mzkit";

print("DEMO of formula calculator function in R# language.");

const H2O = formula::scan("H2O");
const D_glucose = formula::scan("C6H12O6"); 

print(H2O);
print(D_glucose);

const diglucose = D_glucose * 2 - H2O;

cat("\n\n");

print("diglucose:");
print(diglucose);

print("the composed formula:");
print(toString(diglucose));

const rhamnoside = formula::scan("C6H12O5");

cat("\n\n");

const sophorose = formula::scan("C12H22O11");

print("glucose:");

const mono_glucose = (sophorose + H2O) / 2;

print( mono_glucose );
print( toString(mono_glucose) );