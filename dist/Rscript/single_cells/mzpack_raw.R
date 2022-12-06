# require(mzkit);

imports "mzweb" from "mzkit";
imports "mzPack" from "mzkit";

raw_files = list.files("T:\单细胞代谢实验数据\AP-MALDI", pattern = "*.xml");

print(basename(raw_files));

cell_scans = [];

for(path in raw_files) {
	raw = open_mzpack.xml(path, prefer = "mzxml");
	cell_scans = append( cell_scans , [raw]::MS);
	
	print(basename(path));
}

print(cell_scans);

packData(cell_scans, pack.singleCells = TRUE)
|> write.mzPack(file = "T:\单细胞代谢实验数据\AP-MALDI_single_cell_metabolism_SCM_negative.mzPack")
;