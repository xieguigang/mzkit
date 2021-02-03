imports ["GCMS", "Linears"] from "mzkit.quantify";
imports "visualPlots" from "mzkit.quantify";
imports "assembly" from "mzkit";

# get accepted command line arguments:
const calFolder as string  = ?"--cal"  || stop("you must provides a reference samples for linear fitting!");
const MSLIons as string    = ?"--ions" || stop("the ions data in MSL data format must be provided!");
const sampleData as string = ?"--data" || stop("no samples data!");
const output_dir as string = ?"--out"  || `${dirname(sampleData)}/quantify/`;

sink(file = `${output_dir}/run.log`);

const calfiles = list.files(calFolder, pattern = "*.mzML");
const contents = parseContents(calfiles);

print("contents of reference samples in ppb unit:");
str(contents);

const table = contentTable(read.msl(MSLIons, "Minute"), contents, IS = "IS");
const ions  = as.quantify.ion(read.msl(MSLIons, "Minute"));
const sim   = ScanIonExtractor(ions, peakwidth = [8, 16]);

print("read ions raw data and run linear fitting:");

const cal     = lapply(calfiles, function(path) peakRaw(sim, read.raw(path)), names = basename);
const linears = linear_algorithm(table, maxDeletions = 2) :> linears(unlist(cal));

print("Linear modeeling result of your ions data in reference samples:");
cat("\n");

for (line in linears) {
	print(line);

	const linear_bitmap as string = `${output_dir}/linears/${as.object(line)$name}.png`;
	
	line 
	:> visualPlots::standard_curve(gridFill = "white") 
	:> bitmap(file = linear_bitmap)
	;
}

cat("\n");

const quantify = sampleData 
:> list.files(pattern = "*.mzML") 
:> append(calfiles) 
:> sapply(function(file) {
	print("Run quantification of sample data file:");
	print(file);
	
	linears :> quantify(
		ions             = sim :> peakRaw(read.raw(file)), 
		integrator       = "SumAll", 
		fileName         = file, 
		baselineQuantile = 0
	);
});

print("job done!");
print("dumping sample quantification result data:");

result(quantify)     :> write.csv(file = `${output_dir}/quantify.csv`);
scans.X(quantify)    :> write.csv(file = `${output_dir}/rawX.csv`);
lines.table(linears) :> write.csv(file = `${output_dir}/linears.csv`);
ionPeaks(quantify)   :> write.ionPeaks(file = `${output_dir}/ionPeaks.csv`);

for(ion in ions) {
	const linear_tableoutput = `${output_dir}/linears/${as.object(ion)$id}.csv`;

	if (as.object(ion)$id != "IS") {
		points(linears, ion) 
		:> write.points(file = linear_tableoutput)
		;
	}	
}

print("create linear report:");

const plotRaw  = lapply(calfiles, function(path) peakRaw(sim, read.raw(path), chromatogramPlot = TRUE), names = basename);
const plotIons = lapply(ions, function(ion) {
	const ion_id as string = as.object(ion)$id;
	const filesData = lapply(plotRaw, file -> file[[ion_id]]);
	
	filesData;
}, names = ion -> as.object(ion)$id)
;

print("output html report...");

report.dataset(linears, quantify, ionsRaw = plotIons)
:> html()
:> writeLines(con = `${output_dir}/index.html`)
;

print("all job done!");

sink();