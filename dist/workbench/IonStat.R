require(mzkit);
require(ggplot);

imports ["MSI", "mzweb"] from "mzkit";

options(memory.load = "max");

file = ?"--file" || stop("no raw data file!");

print(file);

file 
|> open.mzpack()
|> ionStat()
|> as.data.frame()
|> write.csv(file = `${dirname(file)}/${basename(file)}-ionstat.csv`)
;

# bar plot of the pixels stat
bitmap(file = `${dirname(file)}/${basename(file)}-ionstat.png`) {
	
	

}
