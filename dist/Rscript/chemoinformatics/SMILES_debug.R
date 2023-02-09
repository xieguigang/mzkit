require(mzkit);

#' The chemical formulae toolkit
imports "formula" from "mzkit";

Trigonelline="C[N+]1=CC=CC(=C1)C(=O)[O-]";

struct = Trigonelline
|> formula::parseSMILES() 
;

print(Trigonelline);
print(as.formula(struct)); 
print(formula::atoms(struct));