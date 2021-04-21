require(mzkit);

imports "assembly" from "mzkit";

# title: Summary of the mzXML raw scan numbers
#
# description: 
# Summary of the mzXML raw scan numbers: counting 
# of MS1 scans and MS2 scans.
#

# docker run -it -v "$PWD:$PWD" -w "$PWD" dotnet:npsearch Rscript  ./summary.R

[@info "A folder path that contains the mzXML raw data files."]
const folder as string = ?"--mzXML" || "./";

[@info "the file path of the summary result table output."]
const out as string = ?"--out" || `${folder}/summary.csv`;

let countIndex as function(index) {
	const totalScans = length(index$indexId);
	const countMs2   = length(index$Ms2Index);
	const countMs1   = totalScans - countMs2;
	
	print(index$fileName);
	
	list(
		fileName = basename(index$fileName),
		Ms1      = countMs1,
		MS2      = countMs2,
		allScans = totalScans
	);
}

const summaryList = folder
:> list.files(pattern = "*.mzXML")
:> lapply(load_index, names = basename)
:> lapply(as.object)
:> lapply(countIndex)
;

const summary = data.frame(
	fileName = sapply(summaryList, file -> file$fileName),
	Ms1      = sapply(summaryList, file -> file$Ms1),
	MS2      = sapply(summaryList, file -> file$MS2),
	allScans = sapply(summaryList, file -> file$allScans),
);

print("View of the raw data file summary data:");
print(summary);

write.csv(summary, file = out);