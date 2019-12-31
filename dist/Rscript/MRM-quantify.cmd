@echo off

SET R="../bin/R#.exe"

SET CAL="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\Data20190522liaoning-Cal"
SET data="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\Data20190222"
SET MRM="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\MetaCardio_STD_v5.xlsx"
SET output="D:\biodeep\biodeepDB\smartnucl_integrative\mzML\output"

%R% ./mrm_quantify.R --Cal %CAL% --data %data% --MRM %MRM% --export %output% #--debug


pause