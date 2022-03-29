imports "app" from "ServiceHub";

#' Run MS-imaging ansy backend
#'

[@info "the tcp port for run debugging in VisualStudio."]
[@type "integer"]
const debugPort as string  = ?"--debug" || NULL;
const heartbeats as string = ?"--heartbeats" || NULL;

options(memory.load = "max");

if (!is.empty(heartbeats)) {
	app::listen.heartbeat( port = heartbeats );
}

app::run(service = "MS-Imaging", debugPort = debugPort);