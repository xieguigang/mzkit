@echo off

SET rclass="D:\biodeep\biodeepdb_v3\KEGG\reaction_class"
SET rda="D:\MassSpectrum-toolkits\Rscript\R\MetaDNA\data\metaDNA_kegg.rda"
SET compounds="D:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd"

CALL "bin/KEGG_MetaDNA.exe" /compile /br08201 %rclass% /KEGG_cpd %compounds% /out %rda%