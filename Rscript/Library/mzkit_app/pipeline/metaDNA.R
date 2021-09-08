# run metaDNA annotation on raw data file

const raw as string    = ?"--raw"       || stop("no mzXML/mzML raw data file is provided!");
const output as string = ?"--outputdir" || `${dirname(raw)}/${basename(raw)}-metadna/`;