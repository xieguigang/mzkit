require(mzkit);

imports "annotation" from "mzkit";

adducts = [
	"[M]+",
	"[M+H]+",
	"[M-Cl]+",
	"[M+Na]+",
	"[M+H-H2O]+"
];

print(assert.adducts("CH2O", adducts));