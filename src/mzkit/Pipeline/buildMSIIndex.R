imports "task" from "Mzkit_win32.Task";

[@info "the file path of the imzML raw data file to create cache index."]
[@type "*.imzML"]
const imzML as string = ?"--imzML" || stop("no raw data file provided!");
