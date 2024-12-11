require(mzkit);

[@info "the file path to the rawdata file or a directory folder path that contains multiple rawdata files. supported formats: mzML,mzXML,imzML,raw,mzPack"]
let rawdata = ?"--raw" || stop("no rawdata file was provided!");
let is_dir  = dir.exists(rawdata);
[@info "the file path of the dump text file for a single file input, or a directory path for output multiple rawdata file."]
let outfile = ?"--out" || {
    if (is_dir) {
        rawdata;
    } else {
        `${dirname(rawdata)}/${basename(rawdata)}.txt`;
    }
};

let dump_ascii = function(rawfile, cachefile) {
    let rawdata = open.mzpack(rawfile,verbose =TRUE);
    let ms1 = [rawdata]::MS;

    write.text_cache(ms1, 
        file = cachefile, 
        tabular = TRUE);
}

if (is_dir) {
    let rawfiles = list.files(rawdata, pattern = ["*.mzXML","*.mzML","*.raw","*.imzML"]);
    let names = basename(rawfiles);

    print("dumping raw data files as ascii text cache data:");
    print(names);

    for(let file in tqdm(as.list(rawfiles, names = names))) {
        dump_ascii(file, `${outfile}/${basename(file)}.txt`);
    }

} else {
    dump_ascii(rawdata, outfile);
}

