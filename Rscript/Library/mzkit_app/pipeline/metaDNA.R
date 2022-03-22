require(mzkit);

imports "mzweb" from "mzkit";
imports "metadna" from "mzDIA";

# run metaDNA annotation on raw data file

[@info "the LC-MS raw data file for run metaDNA data annotation."]
const raw as string    = ?"--raw"       || stop("no mzXML/mzML/mzPack raw data file is provided!");
[@info "a data directory for output the result plot files."]
const output as string = ?"--outputdir" || `${dirname(raw)}/${basename(raw)}-metadna/`;

sampleData = raw
|> open.mzpack()
|> ms2_peaks()
;

algorithm = metadna()
|> metadna::range(["[M]+","[M+H]+","[M+H-H2O]+","[M+H-2H2O]+"])
|> metadna::load.kegg( kegg =  )
|> metadna::load.kegg_network( links =  )
|> metadna::load.raw( sample = sampleData )
;

