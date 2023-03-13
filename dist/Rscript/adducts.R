require(mzkit);

imports "annotation" from "mzkit";

adducts = [
	"[M]+",
	"[M+2H2O]+",
	"[M-Cl]+",
	"[M+Na]+",
	"[M+2Na-K]+",
	"[M+H-H2O]+"
];

print(assert.adducts("CH2O", adducts));