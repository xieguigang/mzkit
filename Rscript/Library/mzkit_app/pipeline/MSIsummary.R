require(mzkit);

imports ["MSI", "mzweb"] from "mzkit";

options(memory.load = "max");

for(file in ["V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\a\MSI.mzPack",
"V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney1\b\MSI.mzPack",
"V:\project\项目以外内容\空间代谢组研发_2022年\多切片比较\kidney2\MSI.mzPack"]) {

	print(file);

	file 
	|> open.mzpack()
	|> ionStat()
	|> as.data.frame()
	|> write.csv(file = `${dirname(file)}/ionstat.csv`)
	;
}