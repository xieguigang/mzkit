imports "package_utils" from "devkit";

package_utils::attach("D:\mzkit\Rscript\Library\mzkit_app");

require(mzkit);

imports "MSI" from "mzkit";
imports "mzweb" from "mzkit";

"D:\mzkit\DATA\test\HR2MSI mouse urinary bladder S096 - Figure1.cdf"
|> open.mzpack()
|> write.imzML(file = `${@dir}/HR2MSI mouse urinary bladder S096 - Figure1.imzML`)
;