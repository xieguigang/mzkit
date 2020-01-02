imports "mzkit.formula" from "mzkit.dll";
imports "igraph" from "R.graph.dll";

setwd(!script$dir);

read.KCF("butyryl-CoA.txt") 
# read KEGG molecular model
# and then create network graph
# from the KCF model
:> KCF.graph 
# then we can save the KCF into
# network graph tables
:> save.network(file = "butyryl-CoA");