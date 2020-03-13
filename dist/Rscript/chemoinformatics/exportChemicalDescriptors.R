imports "mzkit.formula" from "mzkit";

using pubchem as open.descriptor.db(dbFile = "D:\biodeep\SDF.description.db") {
	pubchem
	:> descriptor.matrix(cid = 1:1000000)
	:> write.csv(file = "D:\biodeep\pubchem.csv")
	;
}