require(mzkit);

# controls of the server memory usage in the data analysis workflow.
options(memory.load = "max");

#' the spectrum tree reference library tools
imports "spectrumTree" from "mzkit";

rawdir = "E:\reference_ms\DIA\pos";
graph_pack = `${dirname(rawdir)}/lib.${basename(rawdir)}`;

const stdlib = spectrumTree::new(graph_pack, type = "Pack");

for(dir in list.dirs(rawdir,recursive = FALSE)) {
    kegg_id = basename(dir);

    print(dir);
    print(kegg_id);
    
}