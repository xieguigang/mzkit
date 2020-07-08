imports "ProteoWizard" from "mzkit";

if (!msconvert.ready()) {
	options(ProteoWizard = (?"--bin" || stop("ProteoWizard pipeline is not ready!")));
}

let inputDir = ?"--in"  || stop("no input data content was provided!");
let saveOut  = ?"--out" || `${inputDir}.mzML/`;

print("the required input data comes from folder:");
print(inputDir);

inputDir
:> list.files
:> convert.thermo.raw(output = saveOut, filetype = "mzML", parallel = TRUE)
;