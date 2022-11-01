imports "package_utils" from "devkit";

package_utils::attach("D:\mzkit\Rscript\Library\mzkit_app");

require(mzkit);

imports "mzweb" from "mzkit";
imports "MSI" from "mzkit";

const rawfile = "C:\Users\lipidsearch\Downloads\CleanSample.mzPack";

options(memory.load = "max");

using file as file("C:\Users\lipidsearch\Downloads\data.csv") {
	rawfile
	|> open.mzpack()
	|> pixelMatrix(file)
	;
}

