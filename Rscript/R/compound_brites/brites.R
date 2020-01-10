imports "kegg.brite" from "kegg_kit.dll";

setwd(!script$dir);

let brites <- list();

for(id in ["br08001","br08002","br08003","br08005","br08006","br08007","br08008","br08009","br08010","br08021"]) {
	brites[[id]] <- id :> brite.parse :> brite.as.table;
	brites[[id]] :> write.csv(file = `./${id}.csv`);
	   
	print(id);
}

