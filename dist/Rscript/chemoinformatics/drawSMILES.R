imports "formula" from "mzkit";
imports "ChemicalDraw" from "mzplot";

setwd(@dir);

bitmap(file = "./SMILES.png") {
	"CC(=O)OC1=CC=CC=C1C(=O)O"
	|> formula::parseSMILES()
	|> as.kcf()
	|> KCFDraw()
	;
}