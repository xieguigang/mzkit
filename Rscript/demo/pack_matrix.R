require(mzkit);

imports "geneExpression" from "phenotype_kit";
imports "MSI" from "mzkit";

const source = ?"--raw" || stop();
const m = as.data.frame(geneExpression::load.expr0(source));
const save = `${dirname(source)}/${basename(source)}.mzPack`;

m 
|> pack_matrix(noise.cutoff = 0)
|> write.mzPack(
    file = save
);
