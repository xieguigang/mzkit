require(ProteoWizard);

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

for(range in 30:1000 step 120) {
let filters = [filter.msLevel("1-2"), filter.scanTime(range, range + 120)];
let time.output = `${output}/${range}-${range+120}/`

convert.thermo.raw(raw, time.output, "mzXML", filters);
}

