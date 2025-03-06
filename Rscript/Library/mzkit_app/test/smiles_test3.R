require(mzkit);
require(igraph);

imports "SMILES" from "mzkit";
imports "visualizer" from "igraph";
imports "layouts" from "igraph";

# Toluene
let formula = SMILES::parse("CC1=CC=CC=C1");

print(as.data.frame(formula));
print([formula]::EmpiricalFormula);
print(as.formula(formula));

# Biphenyl
let formula = SMILES::parse("C1=CC=C(C=C1)C1=CC=CC=C1",strict =FALSE);

print(as.data.frame(formula));
print([formula]::EmpiricalFormula);
print(as.formula(formula));

setwd(@dir);

bitmap("./biphenyl.png") {
    plot(as.graph(formula) |> layout.springForce(showProgress=FALSE));
}

