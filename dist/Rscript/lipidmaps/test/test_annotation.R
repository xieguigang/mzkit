require(mzkit);

imports ["formula", "math", "data"] from "mzkit";
imports "visual" from "mzplot";

setwd(@dir);

input = read.csv("./LSM(d16-1)+H.csv", row.names = NULL);
annos = read.csv("lipids.csv", row.names = NULL);

print(annos);
formula::registerAnnotations(annos, debug = FALSE);

print(input);

bitmap(file = "./LSM(d16-1)+H.png") {

	input[, "annotation"] = NULL;
	ms2 = input
	|> libraryMatrix(, parentMz = 437.3153)
	|> peakAnnotations(massDiff = 0.005, adducts = math::precursor_types(["[M+H]+","[M+H-2H2O]+"]))
	;

	print("view of the predicted ion fragment annotations:");
	print(as.data.frame(ms2));

	plot(ms2, title = "LSM(d16:1)+H", label.intensity = 0)
	;

}

input = read.csv("./Cer(m17-1_25-2)+H.csv", row.names = NULL);

print(input);

bitmap(file = "./Cer(m17-1_25-2)+H.png") {

	input[, "annotation"] = NULL;
	ms2 = input
	|> libraryMatrix(, parentMz = 630.618455)
	|> peakAnnotations(massDiff = 0.005, adducts = math::precursor_types(["[M+H]+","[M-H2O]+"]))
	;

	print("view of the predicted ion fragment annotations:");
	print(as.data.frame(ms2));

	plot(ms2, title = "LSM(d16:1)+H", label.intensity = 0)
	;

}