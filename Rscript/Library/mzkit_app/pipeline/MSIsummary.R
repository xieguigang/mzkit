require(mzkit);

imports ["MSI", "mzweb"] from "mzkit";

options(memory.load = "max");

setwd("V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较");

# for(file in ["V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\a\MSI.mzPack",
# "V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\b\MSI.mzPack",
# "V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney2\MSI.mzPack"]) {

	# print(file);

	# file 
	# |> open.mzpack()
	# |> ionStat()
	# |> as.data.frame()
	# |> write.csv(file = `${dirname(file)}/ionstat.csv`)
	# ;
# }

files = ["V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\a\MSI.mzPack",
"V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\b\MSI.mzPack",
"V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney2\MSI.mzPack"];

mat = files 
|> lapply(path -> open.mzpack(path), names = ["kidney1-a","kidney1-b","kidney2"])
|> ions_jointmatrix()
;

for(name in colnames(mat)) {
	v = mat[, name];
	v[v < 500] = 0;
	
	mat[, name] = v;
}

i = sapply(1:nrow(mat), i -> any(as.integer(unlist(mat[i,, drop = TRUE ])) > 0) );

mat = mat[ i , ];

print(mat, max.print = 13);

write.csv(mat, file = "./jointMatrix.csv");