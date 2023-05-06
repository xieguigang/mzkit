require(mzkit);

imports "hmdb_kit" from "mzkit";
imports "spectrumTree" from "mzkit";
imports "math" from "mzkit";

# script for create hmdb spectral reference database
const repo = ?"--repo" || stop("A directroy path to the hmdb spectral files must be provided!");
const save_dir = ?"--outputdir" || stop("A directory path for save the spectrum data file must be provided!");
const cachedir = ?"--hmdb.local_cache" || `${@dir}/.cache/`;

const libpos = spectrumTree::new(`${save_dir}/lib.pos.pack`, type = "Pack");
const libneg = spectrumTree::new(`${save_dir}/lib.neg.pack`, type = "Pack");
const raw_stream = repo 
|> read.hmdb_spectrals(hmdbRaw = FALSE) 
|> math::centroid()
;

for(spectral in raw_stream) {
    const hmdb_id = [spectral]::scan;
    const metabolite = hmdb_kit::get_hmdb(hmdb_id, cache.dir = cachedir, tabular = TRUE);

    # skip of the missing data
    if (!is.null(metabolite)) {
        const formula = [metabolite]::chemical_formula;
        const name    = [metabolite]::name;

        if ([spectral]::precursor_type == "[M]+") {
            libpos |> spectrumTree::addBucket(spectral, uuid = hmdb_id, formula = formula, name = name);
        } else {
            libneg |> spectrumTree::addBucket(spectral, uuid = hmdb_id, formula = formula, name = name);
        }
    }
	
	NULL;
}

close(libpos);
close(libneg);
