imports "package_utils" from "devkit";

package_utils::attach("E:\mzkit\Rscript\Library\mzkit_app");

require(mzkit);

imports "mzweb" from "mzkit";
imports "MsImaging" from "mzplot";

"E:\demo\HR2MSI mouse urinary bladder S096.mzPack"
|> open.mzpack()
|> viewer()
|> write.mzImage(file = `${@dir}/demo.mzImage`)
;