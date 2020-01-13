imports "mzkit.assembly" from "mzkit.dll"; 

let mzxml as string = ?"--mzxml";
let mgf as string   = ?"--out" || `${dirname(mzxml)}/${basename(mzxml)}.mgf`;
let ms1 as boolean  = ?"--and.ms1";

if (ms1) {
	print(`Ms1 peaks will also dump from raw file: ${basename(mzxml)} to target mgf file.`);
}

mzxml.mgf(mzxml, FALSE, ms1) 
:> centroid 
:> write.mgf(file = mgf);