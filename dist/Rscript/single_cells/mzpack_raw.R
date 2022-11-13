require(mzkit);

imports "mzweb" from "mzkit";

raw_files = list.files("P:\20221113_29_single_cell\20221113_29_single_cell_metabolism_SCM", pattern = "*.xml");

print(basename(raw_files));

cell_scans = [];

for(path in raw_files) {
	raw = open_mzpack.xml(path, prefer = "mzml");
	cell_scans = cell_scans << [raw]::MS;
}

print(cell_scans);

