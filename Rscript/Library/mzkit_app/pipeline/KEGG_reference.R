require(mzkit);
require(GCModeller);

# controls of the server memory usage in the data analysis workflow.
options(memory.load = "max");

#' the spectrum tree reference library tools
imports "spectrumTree" from "mzkit";
imports "spectrumPool" from "mzDIA";

# "E:\reference_ms\DIA\neg"
rawdir = ?"--rawdir" || stop("no raw data source was provided!");
graph_pack = `${dirname(rawdir)}/lib.${basename(rawdir)}`;
kegg_list = GCModeller::kegg_compounds(rawList = TRUE); 
kegg_id = [kegg_list]::entry;
kegg_list = lapply(kegg_list, x -> x, names = kegg_id);

print(names(kegg_list));
# print(kegg_list@formula);

const stdlib = spectrumTree::new(graph_pack, type = "Pack");

for(dir in list.dirs(rawdir,recursive = FALSE)) {
    kegg_id = basename(dir);
    raw = open.mzpack(`${dir}/reference.mzPack`) 
    |> ms2_peaks(tag.source = FALSE) 
    |> mzkit::get_representives(top_n = 5, 
                                   mzdiff = 0.3,
                                   intocutoff = 0.05,
                                   equals = 0.9);

    print(dir);
    print(kegg_id);

    # do spectrum clustering and then get the max cluster
    metabo = kegg_list[[kegg_id]];

    if (![is.null(metabo) || is.null(raw)]) {
        stdlib |> spectrumTree::addBucket(
            x = spectrumPool::set_conservedGuid(raw, prefix = kegg_id),
            ignore_error = TRUE,
            uuid = kegg_id,
            formula = [metabo]::formula
        );
    }
}

close(stdlib);