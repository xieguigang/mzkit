@echo off

SET rda="D:\MassSpectrum-toolkits\Rscript\R\MetaDNA\data\KEGG_meta.rda"
SET compounds="D:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd"

CALL "bin/KEGG_MetaDNA.exe" /KEGG.meta /kegg %compounds% /out %rda%