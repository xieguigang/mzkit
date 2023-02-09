imports "package_utils" from "devkit";

package_utils::attach("E:\mzkit\Rscript\Library\mzkit_app");

require(mzkit);

imports "formula" from "mzkit";


smiles_str = "CCC(CC)COC(=O)C(C)NP(=O)(OCC1C(C(C(O1)(C#N)C2=CC=C3N2N=CN=C3N)O)O)OC4=CC=CC=C4";
formula = "C27H35N6O8P";

smiles_str
|> parseSMILES()
|> as.formula()
|> print()
;