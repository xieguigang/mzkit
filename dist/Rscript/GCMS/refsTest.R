imports "GCMS" from "mzkit.quantify";
imports "assembly" from "mzkit";

const contents = parseContents(list.files("F:\rawdata\mzML\cal", pattern = "*.mzML"));

str(contents);

const table = contentTable(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"), contents, IS = "IS");

const ions = as.quantify.ion(read.msl("F:\rawdata\mzML\targets-scfa.MSL", "Minute"));

const sim = SIMIonExtractor(as.quantify.ion(ions), peakwidth = [3,6]);

