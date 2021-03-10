imports ["assembly", "mzweb", "chromatogram"] from "mzkit";

const rawfiles as string = ?"--data" || stop("no input data!");
const save as string     = ?"--save" || `${dirname(rawfiles)}/chromatogram.CDF`;

let load_overlaps as function(files as string) {
	print("Loading raw data files");
	print(basename(files));
	
	files :> lapply(function(path) {
		cat("*");
	
		path 
		:> raw.scans 
		:> load.chromatogram
		;
		
	}, names = path -> basename(path))
	:> overlaps
	;
}

const overlaps = rawfiles 
:> list.files(pattern = "*.mz*ML") 
:> load_overlaps 
;

print(overlaps);

write.pack(overlaps, cdf = save);