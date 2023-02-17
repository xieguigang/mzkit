require(biodeepdb_v3);

setwd(@dir);

id = readLines("./kegg_ids.txt");
db = readBiodeepDb();
biodeepid = mapping(db, id, type = "kegg");
kegg_meta = getByBiodeepID(db, biodeepid);

class = [kegg_meta]::class;

summary = table(class);

print(summary);

write.csv(summary, file = "./class.csv");
