require(mzkit);

[@info "the csv table file should contains at least two data field: name and formula"]
let metabolites = ?"--targets" || stop("A csv table data file that contains the target name and target formula is required!");
let rawdir = ?"--rawdir" || stop("A directory that contains the raawdata file is required!");
[@info "the rawdata polarity mode, should be value 1 for positive and -1 for nagative"]
let ion_mode = ?"--ion" || 1;
let outputdir = ?"--outputdir" || file.path(rawdir, "inspect_" & basename(metabolites));

metabolites = read.csv(metabolites, row.names = NULL, check.names = FALSE);

print("view of the metabolites target for make inspect:");
print(metabolites, max.print = 13);

let rawdata = list.files(rawdir, pattern = ["*.mzXML", "*.mzML"]) 
|> lapply(path -> open.mzpack(path), 
    names = path -> basename(path)
)
;

for(let meta in as.list(metabolites, byrow = TRUE)) {
    let [name, formula] = meta$formula;
    let exact_mass = formula::eval(formula);
}