imports ["GCMS", "Linears"] from "mzkit.quantify";
imports "assembly" from "mzkit";

const calFolder as string  = ?"--cal"  || stop("you must provides a reference samples for linear fitting!");
const MSLIons as string    = ?"--ions" || stop("the ions data in MSL data format must be provided!");
const sampleData as string = ?"--data" || stop("no samples data!");
const output_dir as string = ?"--out"  || `${dirname(sampleData)}/quantify/`;

const calfiles = list.files(calFolder, pattern = "*.mzML");
const contents = parseContents(calfiles);

print("contents of reference samples in ppb unit:");
str(contents);

const table = contentTable(read.msl(MSLIons, "Minute"), contents, IS = "IS");
const ions  = as.quantify.ion(read.msl(MSLIons, "Minute"));
const sim   = SIMIonExtractor(ions, peakwidth = [3,6]);

print("read ions raw data and run linear fitting:");

const cal     = lapply(calfiles, function(path) peakRaw(sim, read.raw(path)), names = basename);
const linears = linear_algorithm(table) :> linears(unlist(cal));

print("Linear modeeling result of your ions data in reference samples:");
cat("\n");

for (line in linears) {
	print(line);
}

cat("\n");

const quantify = sampleData 
:> list.files(pattern = "*.mzML") 
:> append(calfiles) 
:> sapply(function(file) {
	print("Run quantification of sample data file:");
	print(file);
	
	linears :> quantify(sim :> peakRaw(read.raw(file)), fileName = file);
});

print("job done!");
print("dumping sample quantification result data:");

result(quantify)     :> write.csv(file = `${output_dir}/quantify.csv`);
scans.X(quantify)    :> write.csv(file = `${output_dir}/rawX.csv`);
lines.table(linears) :> write.csv(file = `${output_dir}/linears.csv`);
ionPeaks(quantify)   :> write.ionPeaks(file = `${output_dir}/ionPeaks.csv`);

for(ion in ions) {
	if (as.object(ion)$id != "IS") {
		points(linears, ion) 
		:> write.points(file = `${output_dir}/linears/${as.object(ion)$id}.csv`)
		;
	}	
}