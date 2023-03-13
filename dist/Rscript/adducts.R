require(mzkit);

imports "annotation" from "mzkit";

adducts = [
	"[M]+",
	"[M+3H2O+Na]+",
	"[M-Cl]+",
	"[M+Na]+",
	"[M+2Na-K]+",
	"[M+H-H2O]+"
];

print(assert.adducts("CH2O", adducts));