require(GCModeller);

imports "geneExpression" from "phenotype_kit";

let raw = load.expr(relative_work("msn.csv"));

raw = impute_missing(raw, by.features=TRUE);
raw = relative(raw, median = TRUE);

write.expr_matrix(raw, file = relative_work("msn_norm.csv") ,
                                 id ="MSn m/z");