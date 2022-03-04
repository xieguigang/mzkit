require(mzkit);
require(ggplot);

imports ["MSI", "mzweb"] from "mzkit";

options(memory.load = "max");

file = ?"--file" || stop("no raw data file!");

print(file);

data = file 
|> open.mzpack()
|> ionStat()
|> as.data.frame()
;

data[, "percentage"] = data[, "pixels"] / max(data[, "pixels"]) * 100;
data = data[ order(data[, "pixels"], decreasing = TRUE) ,  ];

write.csv(data, file = `${dirname(file)}/${basename(file)}-ionstat.csv`);

# bar plot of the pixels stat
bitmap(file = `${dirname(file)}/${basename(file)}-ionstat.png`) {
	
	data[, "log(pixels)"] = log(data[, "pixels"]);
	
	print(data, max.print = 13);


	ggplot(data, aes(x = "log(pixels)"), padding = "padding:250px 600px 250px 300px;", size = [3600, 2100])
	 + geom_histogram(bins = 64,  color = "steelblue")
	 + ggtitle("Ion pixel numbers")
	        + scale_x_continuous(labels = "F2")
       + scale_y_continuous(labels = "F0")
	 + theme_default()
	 ;
	

}
