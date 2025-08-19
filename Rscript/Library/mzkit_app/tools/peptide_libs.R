require(mzkit);
require(bson);

imports "DIA" from "mzDIA";

for(let size in 2:5) {
    bson::write_bson(peptide_lib(size), file = relative_work(`peptide_${size}aa.bson`));
}

