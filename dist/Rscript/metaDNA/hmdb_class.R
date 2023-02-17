require(biodeepdb_v3);

options(memory.load = "max");

setwd(@dir);

id = readLines("./kegg_ids.txt");
db = readBiodeepDb();
biodeepid = mapping(db, id, type = "kegg");
kegg_meta = getByBiodeepID(db, biodeepid);

print(length(kegg_meta));

let class = [kegg_meta]::class;
let summary = table(class);

print(summary);

write.csv(summary, file = "./class.csv");

summary = table([kegg_meta]::super_class);

print(summary);

write.csv(summary, file = "./super_class.csv");