@echo off

SET R="../bin/R#.exe"

SET CAL="D:\20191230_WYL_MRM\XBL\raw\20191218cal"
SET data="D:\20191230_WYL_MRM\XBL\raw\20191218sample(UA+NOCA)"
SET MRM="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\MetaCardio_STD_v5.xlsx"
SET output="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\output"

%R% ./mrm_quantify.R --Cal %CAL% --data %data% --MRM %MRM% --export %output% --debug


pause