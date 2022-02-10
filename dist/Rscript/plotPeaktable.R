require(mzkit);

imports "mz_deco" from "mzkit";
imports "visual" from "mzplot";

data = read.xcms_peaks("F:\peaksdata_nofill.csv");
names = [data]::sampleNames;

print(names);

qc = names[names == $"QC\d+"];

print("get qc names:");
print(qc);

QC = data |> peak_subset(qc);

bitmap(file = `${@dir}/QC_peaks.png`) {
	plot(QC);
}