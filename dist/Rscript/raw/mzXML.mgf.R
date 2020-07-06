imports "assembly" from "mzkit"; 

let mzxml as string        = ?"--mzxml"    || stop("no raw mzXML file provided!");
let mgf as string          = ?"--out"      || `${dirname(mzxml)}/${basename(mzxml)}.mgf`;
let ms1 as boolean         = ?"--and.ms1";
let to.centroid as boolean = ?"--centroid";

if (ms1) {
	print(`Ms1 peaks will also dump from raw file: ${basename(mzxml)} to target mgf file.`);
}
if (to.centroid) {
	print("Ms2 data will also be convert to centroid data.");
}

print(`Mgf ions data will be written to: ${mgf}`);

if (to.centroid) {
	mzxml.mgf(mzxml, FALSE, !ms1) 
	:> centroid
	:> write.mgf(file = mgf);
} else {
	mzxml 
	:> mzxml.mgf(
		relativeInto = FALSE, 
		onlyMs2      = !ms1
	 ) 
	:> write.mgf(file = mgf)
	;
}