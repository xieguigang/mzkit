require(mzkit);

imports "formula" from "mzkit";
imports "data" from "mzkit";
imports "chromatogram" from "mzkit";
imports "visual" from "mzplot";

[@info "the csv table file should contains at least two data field: name and formula, adducts field is optional"]
let metabolites = ?"--targets" || stop("A csv table data file that contains the target name and target formula is required!");
let rawdir = ?"--rawdir" || stop("A directory that contains the raawdata file is required!");
[@info "the rawdata polarity mode, should be value 1 for positive and -1 for nagative"]
let ion_mode = ?"--ion" || 1;
let outputdir = ?"--outputdir" || file.path(rawdir, "inspect_" & basename(metabolites));

metabolites = read.csv(metabolites, row.names = NULL, check.names = FALSE);

print("view of the metabolites target for make inspect:");
print(metabolites, max.print = 13);

let rawdata = list.files(rawdir, pattern = ["*.mzXML", "*.mzML"]) 
|> lapply(function(path) {
        let cache = file.path(dirname(path), `${basename(path)}.mzPack`);

        if (!file.exists(cache)) {
            let data = open.mzpack(path);
            write.mzPack(data, file = cache);
            data;
        } else {
            open.mzpack(cache);
        }
    }, 
    names = path -> basename(path)
)
;
let raw_names = names(rawdata);
let adducts = {
    if (as.integer(ion_mode) > 0) {                    
        ["[M+H]+" "[M-H2O+H]+" "[M-2H2O+H]+" "[M+NH4]+" "[M+Na]+" "[2M+H]+" "[M]+"]             
    } else {                
        ["[M-H]-" "[M+COOH]-"];
    }
};

for(let meta in as.list(metabolites, byrow = TRUE)) {
    let [name, formula] = meta;
    let dir = file.path(outputdir, normalizeFileName(name,alphabetOnly=FALSE));
    let exact_mass = formula::eval(formula);
    let adduct_types = {
        if (is.null(meta$adducts)) {
            adducts;
        } else {
            meta$adducts;
        }
    }
    let mz = math::mz(exact_mass, mode = adduct_types);

    for(let i in 1:length(adduct_types)) {
        let xic = lapply(rawdata, function(file) {
            let filename = [file]::source;
            let xic_data = file 
            |> XIC(mz = mz[i], tolerance = "ppm:20") 
            |> toChromatogram(name = filename)
            ;

            return(xic_data);
        });

        xic = unlist(xic) |> chromatogram::overlaps();

        pdf(file = file.path(dir, `XIC-${adduct_types[i]}.pdf`)) {
            plot(xic, size = [2700, 1920],
               title = `${name}_${adduct_types[i]} m/z:${round(mz[i],4)}`, 
               colors = "paper",
               fill = FALSE, padding = "padding:15% 5% 10% 15%;");
        }
    }    
}