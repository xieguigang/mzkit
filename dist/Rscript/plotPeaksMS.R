imports "visual" from "mzkit.plot";
imports "data" from "mzkit";

const name as string = "malvidin-3-O-arabinoside-xylitol-tetraacetoyl-dioxaloyl-malonoyl";
const anno = list(
"330.9937" = "*[Agly]+",
"354.9948" = "[M-acetoyl-acetoyl-acetoyl-oxaloyl-oxaloyl-malonoyl-arabinoside-H2O]+",
"354.9948" = "[M-acetoyl-acetoyl-acetoyl-acetoyl-oxaloyl-oxaloyl-arabinoside-H2O-CO2]+",
"397.0093" = "[M-acetoyl-acetoyl-oxaloyl-oxaloyl-malonoyl-arabinoside-H2O]+",
"397.0093" = "[M-acetoyl-acetoyl-acetoyl-oxaloyl-oxaloyl-arabinoside-H2O-CO2]+",
"541.0429" = "[M-oxaloyl-oxaloyl-arabinoside-CO2]+",
"541.0429" = "[M-acetoyl-acetoyl-malonoyl-arabinoside-H2O]+",
"541.0429" = "[M-acetoyl-acetoyl-acetoyl-arabinoside-H2O-CO2]+",
"833.1482" = "[M-oxaloyl-H2O]+",
"833.1938" = "[M-oxaloyl-H2O]+"
);

const mz as double = [83.0505, 85.029, 105.1347, 119.0495, 126.6511, 147.0446, 
157.0504, 157.5125, 166.7695, 167.0332, 234.9729, 302.0431, 317.0633, 330.9937, 
345.0104, 354.9948, 376.1086, 383.5285, 397.0093, 541.0429, 552.7606, 746.7862, 
746.9024, 833.1482, 833.1938, 995.1931, 995.2228
];
const into as double = [
2517.9119, 2728.3787, 2822.5684, 4655.0566, 2828.2676, 22407.3965, 12553.7939, 
3120.3821, 2803.3037, 67354.1875, 4714.9624, 4135.5259, 7851.895, 3100.303, 3345.5627, 
2845.2083, 3082.9407, 2681.5422, 3692.0933, 13968.8945, 3086.3394, 3247.4729, 4436.0952, 
6358.2314, 2430.4187, 12983.6299, 18796.9766
];

# print(mz);
# print(into);

# names(anno) = round(as.numeric(names(anno)), 4);

str(anno);

const matrix = data.frame(mz = mz, into = into, annotation = sapply(as.character(mz), function(x) {
	if (x in names(anno)) {
		anno[[x]];
	} else {
		"";
	}
}));

print(matrix);

const MS = libraryMatrix(matrix, name);

print(MS);

bitmap(file = `${dirname(@script)}/MS.png`) {
	plot(MS);
}