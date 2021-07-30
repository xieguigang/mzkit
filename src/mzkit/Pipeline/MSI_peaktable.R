imports "task" from "Mzkit_win32.Task";

require(JSON);
require(graphics2D);

# title: export MSI peaktable
# author: xieguigang <xie.guigang@gcmodeller.org>
# description: export MSI peaktable for downstream data analysis

const raw      as string = ?"--raw"     || stop("a raw data file in mzpack format must be provided!");
const regions  as string = ?"--regions" || stop("a rectangle list data in json format should be provided!");
const savepath as string = ?"--save"    || stop("A file path of the table data output must be provided!");

const getRegions as function() {
	const input   = json_decode(readText(regions));
	const regions = lapply(input, r -> rect(r[1], r[2], r[3], r[4], float = FALSE));
		
	regions;
}

using savefile as file(savepath) {
	task::MSI_peaktable(raw, getRegions(), savefile);
}

