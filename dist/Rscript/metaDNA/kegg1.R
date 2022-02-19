# do ms1 search of kegg

require(mzkit);
require(GCModeller);

setwd(@dir);

const input = ?"--list";

mz = input 
|> readLines() 
|> as.numeric()
;

kegg = kegg_compounds(rawList = TRUE)
|> ms1_handler(
	precursors = precursor_types(["[M]+","[M+H]+","[M+Na]+","[M+K]+","[M+H-H2O]+","[M+NH4]+","[M+H-2H2O]+","[M+CH3OH+H]+"]),
	tolerance = "ppm:30"
)
|> ms1_search(mz, unique = TRUE)
|> unlist()
|> as.data.frame()
;

print(kegg, max.print = 13);

write.csv(kegg, file = `${dirname(input)}/kegg.csv`, row.names = FALSE);


