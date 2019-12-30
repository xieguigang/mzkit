require(ProteoWizard);

let sample as string = ?"--samples" || stop("No wiff raw samples data directory provided!");
let output as string = ?"-output" || `${dirname(sample)}/{basename(sample)}.mzML/` ;
let wiff as string   = list.files(sample, "*.wiff");

print(`we have ${length(wiff)} raw files data for convert to mzML files`);
print(basename(wiff, TRUE));

for(raw in wiff) {
	let output.dir = `${output}/{basename(raw)}`;
	let mzML = MRM.mzML(wiff = raw, output = output.dir);
	
	print(`there are ${length(mzML)} samples in raw data '${basename(raw, TRUE)}'`);
	print(mzML);
}