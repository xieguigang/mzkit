imports "task" from "Mzkit_win32.Task";

# title: Build contour layers data cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the mzpack raw data file to create contour layers data."]
[@type "*.mzpack"]
const mzPack as string = ?"--mzPack" || stop("no raw data file provided!");
[@info "the file path of the json result file."]
[@type "filepath"]
const cache as string = ?"--cache" || stop("a cache file path must be provided!");

sleep(1);
task::Ms1Contour(mzPack, cache);