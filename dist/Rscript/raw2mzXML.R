imports "ProteoWizard" from "mzkit";

let sample as string = ?"--samples" || stop("No raw samples data directory provided!");
let output as string = ?"--output"  || `${dirname(sample)}/${basename(sample)}.mzXML/` ;
let raw as string   = list.files(sample, "*.raw");

if (!msconvert.ready()) {
	# missing ProteoWizard configuration data or its configuration value
	# invalids
	# do ProteoWizard configuration via commandline input
	let msconvert = ?"--msconvert" || stop("Missing 'ProteoWizard' configuration or its configuration value is invalids!");
	
	# msconvert is the file path of the 
	# program ``msconvert.exe`` in ProteoWizard.
	options(ProteoWizard = msconvert);
}

print(`we have ${length(raw)} raw files data for convert to mzXML files`);
print(basename(raw, TRUE));

let times = 30:1000 step 120;

print (times);

let doConvert as function(range, delta = 120) {
	let filters = [filter.msLevel("1-2"), filter.scanTime(range, range + delta)];
	let time.output = `${output}/${range}-${range+ delta}/`;

	convert.thermo.raw(raw, time.output, "mzXML", filters);
}

# for(range in times) {
	# doConvert(range);
# }

# doConvert(750, 89);

doConvert(0,1800);

