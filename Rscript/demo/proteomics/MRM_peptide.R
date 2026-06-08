require(bson);
require(mzkit);

#' title: peptide annotation of the MRM ions
#' author: xieguigang <xieguigang@metabolomics.ac.cn>

imports "DIA" from "mzDIA";
imports "metadb" from "mzkit";
imports "MRMLinear" from "mz_quantify";

[@info "the file path of the MRM raw data file."]
let raw = ?"--mzML" || stop("missing required MRM raw data file!");
[@info "the ion polarity mode for the input mzML rawdata file, value should be pos/neg. default is in positive mode."]
let ion_mode = ?"--ion" || "pos";
[@info "the peptide mass library file, value could be peptide_3aa.bson / peptide_3aa.bson / peptide_4aa.bson."]
let libfile = ?"--lib" || relative_work("./peptide_3aa.bson");
[@info "the mass tolerance error for make ion mass matches."]
let mzdiff = ?"--mzdiff" || 0.3;

let s = file(libfile); 
let lib = bson::parse_bson(s, what = "peptide_mass");
let pos = c("[M+H]+","[M+Na]+","[M+K]+","[M+NH4]+");
let neg = c("[M-H]-","[M+Acetate]-","[M+HCOO]-");
let adducts = {
    if (ion_mode == "pos") {
        pos;
    } else {
        neg;
    }
};
let localdb = ms1_handler(lib, precursors = adducts, tolerance = `da:${mzdiff}`);
let ions = extract_ionpairs(raw);

let run_annotation = function(ion) {
    let q1 = [ion]::precursor;
    let q3 = [ion]::product;

    # search by Q1
    let result = localdb |> ms1_search(mz = q1, unique = FALSE);
    let q1_find = localdb 
        |> getMetadata([result]::unique_id) 
        |> unlist()
        ;

    if (length(q1_find) > 0) {
        let q3_db = ms1_handler(peptide_q3(q1_find), precursors = adducts, tolerance = `da:${mzdiff}`);
        # search by Q3
        let result_q3 = q3_db |> ms1_search(mz = q3, unique = FALSE);

        if (length(result_q3) > 0) {
            result = as.data.frame(result_q3);
        } else {
            result = NULL;
        }
    } else {
        result = NULL;
    }
    
    if (nrow(result) > 0) {
        result[,"q1"] = q1;
        result[,"q3"] = q3;

        result[,"intensity"] = NULL;
        result[,"mz"] = NULL;
        result[,"name"] = NULL;
        result[,"mz_ref"] = NULL;
        result[,"ppm"] = NULL;
        result[,"score"] = NULL;
        result[,"index"] = NULL;
    }

    result;
}

let result = bind_rows(lapply(ions, ion -> run_annotation(ion)));

print("view of the annotation search result:");
print(result);

write.csv(result, file = file.path(dirname(raw), `${basename(raw)}.csv`), row.names = FALSE);