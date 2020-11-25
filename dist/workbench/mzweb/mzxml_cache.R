imports "assembly" from "mzkit";
imports "mzweb" from "mzkit";

# convert the raw mzxml file to data cache

let mzxml as string = ?"--mzxml";
let save_cache as string = ?"--save" || `${dirname(mzxml)}/${basename(mzxml)}.mzweb_cache.txt`;

print(save_cache);

mzxml
:> raw.scans
:> load.stream
:> write.cache(file = save_cache)
;