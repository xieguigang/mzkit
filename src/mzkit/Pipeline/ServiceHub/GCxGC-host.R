imports "app" from "ServiceHub";

#' Run MS-imaging ansy backend
#'

[@info "the tcp port for run debugging in VisualStudio."]
[@type "integer"]
const debugPort as string = ?"--debug" || NULL;

options(memory.load = "max");

app::run(service = "GCxGC", debugPort = debugPort);