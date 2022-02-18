require(mzkit);
require(filter);

imports "MsImaging" from "mzplot";
imports "mzweb" from "mzkit";

options(memory.load = "max");

const rawPack as string    = ?"--mzpack"      || stop("A raw data file path is required!");
const mzdiff as double     = ?"--mzdiff"      || 1;
const gridSize as integer  = ?"--grid-size"   || 6;
const densityCut as double = ?"--density-cut" || 0.1;
const outputdir as string  = ?"--outputdir"   || `${dirname(rawPack)}/MSI/`;

const ions = open.mzpack(rawPack) 
|> MeasureMSIions(
	gridSize   = gridSize, 
	mzdiff     = `da:${mzdiff}`, 
	keepsLayer = TRUE, 
	densityCut = densityCut
);
const [mz, density, layer] = ions;

print(head(ions));

for(i in 1:nrow(ions)) %dopar% {
	print("Rendering of the ion layer:");
	print(mz[i]);

	bitmap(file = `${outputdir}/${round(mz[i], 4)}.png`) {
		render(
			data        = layer[i], 
			colorSet    = "viridis:turbo", 
			cutoff      = TrIQ(layer[i], q = 0.45),
			pixelSize   = [2,2],
			defaultFill = "black"
		);
	}
}