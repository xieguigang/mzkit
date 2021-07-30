imports "task" from "Mzkit_win32.Task";

# title: export MSI peaktable
# author: xieguigang <xie.guigang@gcmodeller.org>
# description: export MSI peaktable for downstream data analysis

const raw      as string = ?"--raw"     || stop("a raw data file in mzpack format must be provided!");
const regions  as string = ?"--regions" || stop("a rectangle list data in json format should be provided!");
const savepath as string = ?"--save"    || stop("A file path of the table data output must be provided!");

const getRegions as function() {
	
}

using savefile as file(savepath) {
	task::MSI_peaktable(raw, getRegions(), savefile);
}

