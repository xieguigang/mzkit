imports "GCMS" from "mzkit.quantify";
imports "assembly" from "mzkit";

const contents = parseContents(list.files("F:\rawdata\mzML\cal", pattern = "*.mzML"));

str(contents);

const table = contentTable(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"), contents, IS = "IS");

const ions = as.quantify.ion(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"));

const sim = SIMIonExtractor(ions, peakwidth = [3,6]);

const cal = lapply(list.files("F:\rawdata\mzML\cal", pattern = "*.mzML"), function(path) {
	peakRaw(sim, read.raw(path));
}, names = basename);

print(names(cal));

const linears = linear_algorithm(table) :> linears(unlist(cal));

for (line in linears) {
	print(line);
}

const quantify = sapply(list.files("F:\rawdata\mzML\data", pattern = "*.mzML"), function(file) {
	linears :> quantify( sim :> peakRaw(read.raw(file)), fileName = file);

});

result(quantify) :> write.csv(file = "F:\rawdata\mzML\targets-scfa.quantify.csv");
scans.X(quantify) :> write.csv(file = "F:\rawdata\mzML\targets-scfa.rawX.csv");