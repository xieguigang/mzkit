imports "kegg.brite" from "kegg_kit.dll";

setwd(!script$dir);

let kegg_id as string <- ["br08001","br08002","br08003","br08005","br08006","br08007","br08008","br08009","br08010","br08021"];
let brites <- lapply(kegg_id, id -> id :> brite.parse :> brite.as.table);

for(id in names(brites)) {
	print(id);
	
	# write id file
	brites[[id]] :> write.csv(file = `./${id}.csv`);
}

