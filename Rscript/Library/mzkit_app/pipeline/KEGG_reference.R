require(mzkit);
require(GCModeller);

# controls of the server memory usage in the data analysis workflow.
options(memory.load = "max");

#' the spectrum tree reference library tools
imports "spectrumTree" from "mzkit";
imports "spectrumPool" from "mzDIA";

rawdir = "E:\reference_ms\DIA\neg";
graph_pack = `${dirname(rawdir)}/lib.${basename(rawdir)}`;
kegg_list = GCModeller::kegg_compounds(rawList = TRUE); 
kegg_id = [kegg_list]::entry;
kegg_list = lapply(kegg_list, x -> x, names = kegg_id);

print(names(kegg_list));
# print(kegg_list@formula);

const stdlib = spectrumTree::new(graph_pack, type = "Pack");

for(dir in list.dirs(rawdir,recursive = FALSE)) {
    kegg_id = basename(dir);
    raw = open.mzpack(`${dir}/reference.mzPack`) |> ms2_peaks(tag.source = FALSE);

    print(dir);
    print(kegg_id);

    # do spectrum clustering and then get the max cluster
    metabo = kegg_list[[kegg_id]];

    if (!is.null(metabo)) {
        stdlib |> spectrumTree::addBucket(
            x = spectrumPool::set_conservedGuid(raw),
            ignore_error = TRUE,
            uuid = kegg_id,
            formula = [metabo]::formula
        );
    }
}

close(stdlib);