imports ["igraph.render", "igraph.layouts"] from "R.graph.dll";
imports "mzkit.metadna" from "mzkit.insilicons.dll";

let repo as string = "D:\MassSpectrum-toolkits\src\metadna\test";
let test.model = `${repo}/human_blood.Xml`;

setwd(!script$dir);

test.model
:> read.metadna.infer
:> layout.force_directed
:> render.Plot
:> save.graphics(file = "plot.png");