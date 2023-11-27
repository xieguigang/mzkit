# helper script for imports pubchem kegg compound list into GCModeller package

require(GCModeller);
require(mzkit);

imports "pubchem_kit" from "mzkit";
imports "formula" from "mzkit";
imports "repository" from "kegg_kit";

const hits = read.webquery("E:\\PubChem_compound_text_kegg.xml");

for(metabo in hits) {
    metabo = as.list(metabo);
    str(metabo);
    # stop();

    let names = metabo$cmpdsynonym$cmpdsynonym;

    print(names);

    let kegg_id = names[names == $"C\d{5}"];
    let name = metabo$cmpdname;
    let formula = metabo$mf; 
    let cas_number = names[names == $"\d+([-]\d+){2}"];

    let db = [];
    let id = [];
    let link = [];

    db = append(db, rep("CAS", length(cas_number)));
    id = append(id, cas_number);
    link = append(link, rep("", length(cas_number)));

    print(kegg_id);

    let kegg_compound = repository::compound(
        entry = kegg_id,
        name = name,
        formula = formula,
        exactMass = formula::eval(formula),         
        DBLinks = data.frame(
            db, id, link
        )
    );

    print(xml(kegg_compound));

    stop();
}