require(mzkit);

imports "formula" from "mzkit";

print(canonical_formula(formula::scan("HOH")));
print(canonical_formula(formula::scan("HOHFeCCNPOCClCH66")));
print(canonical_formula(formula::scan("HCRzCN")));
print(canonical_formula(formula::scan("HOHXSCCCCCCCCCCCCCCCCCCCCCC")));