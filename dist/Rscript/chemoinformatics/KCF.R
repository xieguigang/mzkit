imports "mzkit.formula" from "mzkit.dll";

require(igraph);
require(igraph.comparison);

setwd(!script$dir);

let cutoff = 0.8;

# test of the butyrate
let butyrate        <- read.KCF("butyrate.txt")       :> KCF.graph;
let butyryl.CoA     <- read.KCF("butyryl-CoA.txt")    :> KCF.graph;
let X2.butenoyl.CoA <- read.KCF("2-butenoyl-CoA.txt") :> KCF.graph; 

# print(butyrate);
# print(butyryl.CoA);

print("Similarity between [Butyrate] and [Butyryl-CoA]:");
print(graph.jaccard(butyrate, butyryl.CoA, cutoff, topologyCos = TRUE));
print("Similarity between [Butyrate] and [2-Butenoyl-CoA]:");
print(graph.jaccard(butyrate, X2.butenoyl.CoA, cutoff, topologyCos = TRUE));
print("Similarity between [2-Butenoyl-CoA] and [Butyryl-CoA]:");
print(graph.jaccard(X2.butenoyl.CoA, butyryl.CoA, cutoff, topologyCos = TRUE));

# test of the ATP etc
# let ATP <- read.KCF("ATP.txt") :> KCF.graph;
# let AMP <- read.KCF("AMP.txt") :> KCF.graph;

# print(ATP);
# print(AMP);

# print("Similarity between [ATP] and [AMP]:");
# print(graph.jaccard(ATP, AMP, cutoff));