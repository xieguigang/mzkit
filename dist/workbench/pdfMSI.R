require(mzkit);

imports "MsImaging" from "mzplot";

setwd(@dir);

data = "E:\mzkit\DATA\test\HR2MSI mouse urinary bladder S096 - Figure1.cdf";
data = open.mzpack(data);

print(data);

pdf(file = "./MsImaging.pdf") {
	viewer(data)
	|> layer([741.55, 743.54, 798.54], background = "black")
	;
}

pdf(file = "./MsImaging2.pdf") {
viewer(data)
	|> MSIlayer([741.55, 743.54, 798.54])
	|> plot(colorSet = "viridis:plasma", grid.fill = "white")
	;
}
