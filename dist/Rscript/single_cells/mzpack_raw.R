require(mzkit);
require(GCModeller);

imports "mzweb" from "mzkit";
imports "mzPack" from "mzkit";
imports "SingleCells" from "mzkit";
imports "geneExpression" from "phenotype_kit";

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

"T:\单细胞代谢实验数据\AP-MALDI_single_cell_metabolism_SCM_negative.mzPack" 
|> open.mzpack()
|> SingleCells::cell_matrix(raw)
|> geneExpression::write.expr_matrix(
	file = "T:\单细胞代谢实验数据\AP-MALDI_single_cell_metabolism_SCM_negative.csv",
	id = "cell_id"
)
;