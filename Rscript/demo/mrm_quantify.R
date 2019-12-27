imports "mzkit.mrm" from "mzkit.quantitative.dll";

let wiff as string = "T:\test\Data20190522liaoning-Cal";
let mrmInfo as string = "T:\_ref\MetaCardio_STD_v5.xlsx";
let [ions, ref, is] = mrmInfo :> [
	read.ion_pairs("ion pairs"), 
	read.reference("coordinates"), 
	read.IS("IS")
];

let CAL = list.files(wiff, pattern = "*.mzML")
:> wiff.scans(ions,peakAreaMethod= 0, TPAFactors = NULL);

CAL 
:> write.csv(file = "T:\test\L.csv");

ref <- CAL 
:> linears(ref, is) 
:> models;

for(line in ref) {
	print(line);
}

for(mzML in list.files(wiff, pattern = "*.mzML")) {
	let fileName = basename(mzML);
	let peaks = MRM.peaks(mzML, ions, peakAreaMethod= 0, TPAFactors = NULL);
	
	write.csv( peaks, file = `T:\test\peaktables/${fileName}.csv`);
}

wiff = "T:\test\Data20190222";

list.files(wiff, pattern = "*.mzML")
:> wiff.scans(ions,peakAreaMethod= 0, TPAFactors = NULL) 
:> write.csv(file = "T:\test\samples.csv");

