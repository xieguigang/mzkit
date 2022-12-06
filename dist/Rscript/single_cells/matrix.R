require(mzkit);
require(GCModeller);

imports "mzweb" from "mzkit";
imports "mzPack" from "mzkit";
imports "SingleCells" from "mzkit";
imports "geneExpression" from "phenotype_kit";

"T:\单细胞代谢实验数据\AP-MALDI_single_cell_metabolism_SCM_negative.mzPack" 
|> open.mzpack()
|> SingleCells::cell_matrix()
|> geneExpression::write.expr_matrix(
	file = "T:\单细胞代谢实验数据\AP-MALDI_single_cell_metabolism_SCM_negative.csv",
	id = "cell_id"
)
;