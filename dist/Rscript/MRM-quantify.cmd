@echo off

SET R="../bin/R#.exe"

SET CAL="S:\2018\WYL\20191230_WYL_MRM\XBL\raw\20191218cal"
SET data="S:\2018\WYL\20191230_WYL_MRM\XBL\raw\20191218sample(UA+NOCA)"
SET MRM="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\MetaCardio_STD_v5-test.xlsx"
SET output="S:\2018\WYL\20191230_WYL_MRM\XBL\result\20191218sample(UA+NOCA)"

%R% ./mrm_quantify.R --Cal %CAL% --data %data% --MRM %MRM% --export %output% #--debug


REM pause