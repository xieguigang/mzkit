require(mzkit);

imports "data" from "mzkit";
imports "visual" from "mzplot";

name = "[MSn][Scan_3843] FTMS + p ESI d Full ms2 563.1486@hcd30.00 [50.0000-595.0000] M563T290";
data = data.frame(
	mz = [97.0289077758789, 100.20857238769531, 112.05076599121094, 214.63270568847656, 226.081787109375, 227.03094482421875, 323.0516357421875],
	into = [161495.25, 8638.7451171875, 110263.1640625, 13919.138671875, 8425.4365234375, 8676.7412109375, 9799.5654296875]
)
|> libraryMatrix(title = name)
;

svg(file = `${@dir}/plotMs.svg`) {
	plot(data, mirror = TRUE);
}