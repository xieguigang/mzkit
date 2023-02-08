require(mzkit);

#' The chemical formulae toolkit
imports "formula" from "mzkit";

Trigonelline="C[N+]1=CC=CC(=C1)C(=O)[O-]";

Trigonelline 
|> formula::parseSMILES() 
|> as.formula() 
|> print()
;