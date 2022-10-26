imports "package_utils" from "devkit";

package_utils::attach("D:\mzkit\Rscript\Library\mzkit_app");

require(mzkit);

imports "massbank" from "mzkit";


"C:\Users\lipidsearch\Source\MoNA-export-LC-MS_Spectra.msp"
|> read.MoNA()
|> as.vector()
;