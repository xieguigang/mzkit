imports "task" from "Mzkit_win32.Task";

# title: Build indexed MSI cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the mzPack/mgf raw data file to create cache index."]
[@type "*.mzpack;*.mgf"]
const raw as string = ?"--raw" || stop("no raw data file provided!");
[@info "the directory path of the metaDNA output result files."]
[@type "directory"]
const outputdir as string = ?"--save" || stop("a directory path for save result files must be provided!");
const ssid as string = ?"--biodeep_ssid" || stop("a session id for biodeep is required!");

sleep(1);

task::biodeep.session(ssid);
task::metaDNA(raw, outputdir);