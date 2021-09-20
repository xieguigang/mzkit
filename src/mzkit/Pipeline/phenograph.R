imports "task" from "Mzkit_win32.Task";
imports "igraph" from "igraph";

const raw as string       = ?"--raw"     || stop("missing of the raw data file for run MSI data analysis!");
const topN as integer     = ?"--top"     || 10;
const maxDims as integer  = ?"--maxDims" || 256;
const mzErr as string     = ?"--mzdiff"  || "da:0.3";
const savegraph as string = ?"--save"    || `${dirname(raw)}/${basename(raw)}_phenograph/`; 

raw
|> phenograph(topN, maxDims, mzErr)
|> save.network(file = savegraph)
;