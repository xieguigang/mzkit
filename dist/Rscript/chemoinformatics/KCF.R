imports "mzkit.formula" from "mzkit.dll";

require(igraph);
require(igraph.comparison);

setwd(!script$dir);

let cutoff = 0.6;

# test of the butyrate
let butyrate    <- read.KCF("butyrate.txt")    :> KCF.graph;
let butyryl.CoA <- read.KCF("butyryl-CoA.txt") :> KCF.graph;

# print(butyrate);
# print(butyryl.CoA);

print("Similarity between [Butyrate] and [Butyryl-CoA]:");
print(graph.jaccard(butyrate, butyryl.CoA, cutoff));

# test of the ATP etc
let ATP <- read.KCF("ATP.txt") :> KCF.graph;
let AMP <- read.KCF("AMP.txt") :> KCF.graph;

# print(ATP);
# print(AMP);

print("Similarity between [ATP] and [AMP]:");
print(graph.jaccard(ATP, AMP, cutoff));