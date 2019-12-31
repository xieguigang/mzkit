# imports mzkit library modules
imports ["mzkit.mrm", "mzkit.quantify.visual"] from "mzkit.quantify.dll";

# config of the standard curve data files
let wiff as string     = ?"--Cal"    || stop("No standard curve data provides!");
let sample as string   = ?"--data"   || stop("No sample data files provided!");
let MRM.info as string = ?"--MRM"    || stop("Missing MRM information table file!");
let dir as string      = ?"--export" || `${wiff :> trim(" ")}-result/`;

# read MRM, standard curve and IS information from the given file
let [ions, ref, is] = MRM.info :> [
	read.ion_pairs("ion pairs"), 
	read.reference("coordinates"), 
	read.IS("IS")
];

# Get raw scan data for given ions
let CAL = list.files(wiff, pattern = "*.mzML")
:> wiff.scans(
	ions           = ions, 
	peakAreaMethod = 0, 
	TPAFactors     = NULL
);

CAL 
:> write.csv(file = `${dir}/CAL.csv`);

ref <- linears(CAL, ref, is);

# print model summary and then do standard curve plot
let printModel as function(line) {
	# get compound id name
	let id as string = line 
	:> as.object 
	:> do.call("name");
	
	# view summary result
	print(line);
	
	line
	:> standard_curve
	:> save.graphics(file = `${dir}/standard_curves/${id}.png`);
}

for(line in ref) {
	line :> printModel;
}

for(mzML in list.files(wiff, pattern = "*.mzML")) {
	let fileName = basename(mzML);
	let peaks = MRM.peaks(mzML, ions, peakAreaMethod = 0, TPAFactors = NULL);
	
	# save peaktable for given rawfile
	write.csv(peaks, file = `${dir}/peaktables/${fileName}.csv`);
}

wiff <- sample;

list.files(wiff, pattern = "*.mzML")
:> wiff.scans(ions,peakAreaMethod= 0, TPAFactors = NULL) 
:> write.csv(file = `${dir}\samples.csv`);

let scans = [];

for(sample.mzML in list.files(wiff, pattern = "*.mzML")) {
	let peakfile as string = `${dir}/${basename(sample.mzML)}.csv`;
	let result = ref 
		:> sample.quantify(sample.mzML, ions, 0, NULL);
	
	print(basename(sample.mzML));
	
	result 
	:> as.object 
	:> do.call("MRMPeaks") 
	:> write.MRMpeaks(file = peakfile);
	
	scans <- scans << result;
}

print("Sample raw files that we scans:");
print(length(scans));

# save the MRM quantify result
# base on the linear fitting
result(scans)  :> write.csv(file = `${dir}\quantify.csv`);
scans.X(scans) :> write.csv(file = `${dir}\rawX.csv`);
