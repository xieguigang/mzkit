imports ["GCMS", "Linears"] from "mzkit.quantify";
imports "assembly" from "mzkit";

const calfiles = list.files("F:\rawdata\mzML\cal", pattern = "*.mzML");

const contents = parseContents(calfiles);

str(contents);

const table = contentTable(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"), contents, IS = "IS");

const ions = as.quantify.ion(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"));

const sim = SIMIonExtractor(ions, peakwidth = [3,6]);

const cal = lapply(calfiles, function(path) {
	peakRaw(sim, read.raw(path));
}, names = basename);

print(names(cal));

const linears = linear_algorithm(table) :> linears(unlist(cal));

for (line in linears) {
	print(line);
	
	
}

const quantify = sapply(append(list.files("F:\rawdata\mzML\data", pattern = "*.mzML"), calfiles), function(file) {
	print(file);

	linears :> quantify( sim :> peakRaw(read.raw(file)), fileName = file);
});

result(quantify) :> write.csv(file = "F:\rawdata\mzML\targets-scfa.quantify.csv");
scans.X(quantify) :> write.csv(file = "F:\rawdata\mzML\targets-scfa.rawX.csv");

lines.table(linears) :> write.csv(file = "F:\rawdata\mzML\targets-scfa.linears.csv");

for(ion in ions) {
	points(linears, ion) :> write.points(file = `F:\rawdata\mzML\linears/${as.object(ion)$Name}.csv`);
}