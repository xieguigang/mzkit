imports "task" from "Mzkit_win32.Task";

# title: Build mzPack cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the mzML/mzXML/raw raw data file to create mzPack cache file."]
[@type "*.mzML;*.mzXML;*.raw"]
const mzXML as string = ?"--mzXML" || stop("no raw data file provided!");
[@info "the file path of the mzPack cache file."]
[@type "filepath"]
const cache as string = ?"--cache" || stop("a cache file path must be provided!");

sleep(1);
task::cache.mzpack(mzXML, cache);