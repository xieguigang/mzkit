imports "formula" from "mzkit";

# print(as.formula(formula::parseSMILES("CCN(CC)CC")));

let echo as function(SMILES, prompt) {
	print(prompt);

    SMILES = formula::parseSMILES(SMILES);
	
	print(toString(as.formula(SMILES)));
	print(`exact mass: ${eval(as.formula(SMILES))}`);
	print("---------------------------------------");
	
	cat("\n");
}

# echo("CCN(CC)CC", "demo test");

echo("CC",            "ethane CH3CH3");
echo("C=O",           "formaldehyde (CH2O)");
echo("C=C",           "ethene (CH2=CH2)");
echo("O=C=O",         "carbon dioxide (CO2)");
echo("COC",           "dimethyl ether (CH3OCH3)");
echo("C#N",           "hydrogen cyanide (HCN)");
echo("CCO",           "ethanol (CH3CH2OH)");
echo("[H][H]",        "molecular hydrogen (H2)");
echo("C=C-C-C=C-C-O", "6-hydroxy-1,4-hexadiene CH2=CH-CH2-CH=CH-CH2-OH");

echo("CCN(CC)CC",            "Triethylamine C6H15N");
echo("CC(C)C(=O)O",          "Isobutyric acid C4H8O2");
echo("C=CC(CCC)C(C(C)C)CCC", "3-propyl-4-isopropyl-1-heptene C10H20");