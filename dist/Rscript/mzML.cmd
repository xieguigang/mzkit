@echo off

SET R="../bin/R#.exe"

%R% ./wiff2mzML.R --samples "S:\2018\WYL\XBL" --output "S:\2018\WYL\20191230_WYL_MRM\XBL\raw" --debug
REM %R% ./wiff2mzML.R --samples "S:\2018\WYL\MeiZhou" --output "S:\2018\WYL\20191230_WYL_MRM\MeiZhou\raw"