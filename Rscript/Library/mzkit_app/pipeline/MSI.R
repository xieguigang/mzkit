imports "MsImaging" from "mzplot";
imports "mzweb" from "mzkit";

const rawPack as string    = ?"--mzpack"      || stop("A raw data file path is required!");
const mzdiff as double     = ?"--mzdiff"      || 0.1;
const gridSize as integer  = ?"--grid-size"   || 5;
const densityCut as double = ?"--density-cut" || 0.65;

const ions = open.mzpack(rawPack) 
|> MeasureMSIions(
	gridSize   = gridSize, 
	mzdiff     = `da:${mzdiff}`, 
	keepsLayer = TRUE, 
	densityCut = densityCut
);

print(head(ions));

for(i in nrow(ions)) {

}