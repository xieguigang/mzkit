require(mzkit);

imports "massbank" from "mzkit";

let mona = read.MoNA("E:\biodeep\MoNA-export-LC-MS_Spectra.msp",skipSpectraInfo =TRUE);
let metabolites = extract_mona_metabolites(mona);

