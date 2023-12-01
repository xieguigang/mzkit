# helper script for imports pubchem kegg compound list into GCModeller package

require(GCModeller);
require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "formula" from "mzkit";
imports "repository" from "kegg_kit";

const hits = read.webquery("E:\\PubChem_compound_text_drugbank.xml");
# const hits = read.webquery("E:\\PubChem_compound_text_kegg.xml");
const kegg_compounds = list();

for(metabo in hits) {
    metabo = as.list(metabo);
    # str(metabo);
    # stop();

    let names = metabo$cmpdsynonym;

    # print(names);

    let kegg_id = as.character(names[names == $"C\d{5}"]);
    let drugbank_id = names[names == $"DB\d{5}"];

    # print(kegg_id);

    # if ( length(kegg_id) == 0 ) {
    #     next;
    # }
    if (length(drugbank_id) == 0) {
        next;
    }

    # if (length(kegg_id) > 1) {
    #     # stop(kegg_id);
    #     kegg_id = kegg_id[1];
    # }

    let name = metabo$cmpdname;
    let formula = metabo$mf; 
    let cas_number = names[names == $"\d+([-]\d+){2}"];
    let hmdb_id = names[names == $"HMDB\d+"];
    let chebi_id = names[names == $"CHEBI[:]\d+"];
    let chembl_id = names[names == $"CHEMBL\d+"];
    let annotation = metabo$annotation;

    let db = [];
    let id = [];
    let link = [];

    if (nchar(metabo$inchi) > 0) {
        db = append(db, "InChI");
        id = append(id, metabo$inchi);
        link = append(link, "");

        db = append(db, "InChIKey");
        id = append(id, metabo$inchikey);
        link = append(link, "");
    }

    if (nchar(metabo$canonicalsmiles) > 0) {
        db = append(db, "SMILES");
        id = append(id, metabo$canonicalsmiles);
        link = append(link, "");
    }

    db = append(db, rep("KEGG", length(kegg_id )));
    id = append(id, kegg_id );
    link = append(link, rep("", length(kegg_id )));

    # db = append(db, rep("DrugBank", length(drugbank_id)));
    # id = append(id, drugbank_id);
    # link = append(link, rep("", length(drugbank_id)));

    db = append(db, rep("ChEMBL", length(chembl_id)));
    id = append(id, chembl_id);
    link = append(link, rep("", length(chembl_id)));

    db = append(db, rep("ChEBI", length(chebi_id)));
    id = append(id, chebi_id);
    link = append(link, rep("", length(chebi_id)));

    db = append(db, rep("HMDB", length(hmdb_id)));
    id = append(id, hmdb_id);
    link = append(link, rep("", length(hmdb_id)));

    db = append(db, rep("CAS", length(cas_number)));
    id = append(id, cas_number);
    link = append(link, rep("", length(cas_number)));

    db = append(db, "PubChem");
    id = append(id, metabo$cid);
    link = append(link, "");

    if (length(metabo$meshheadings) > 0) {
        db = append(db, rep("MeSH", length(metabo$meshheadings)));
        id = append(id, metabo$meshheadings);
        link = append(link, rep("", length(metabo$meshheadings)));
    }    

    for(keg in drugbank_id ) {
        let kegg_compound = repository::compound(
            entry = keg,
            name = name,
            formula = formula,
            exactMass = formula::eval(formula),         
            remarks = annotation,
            DBLinks = data.frame(
                db, id, link
            )
        );

        # print(xml(kegg_compound));

        kegg_compounds[[keg]] = kegg_compound;
    }



    print(`${drugbank_id } - ${name}`);

    # stop();
    # break;
}

repository::write.msgpack(unlist(kegg_compounds), file = "E:\\compounds\\drugbank.msgpack");