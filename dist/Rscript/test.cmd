@echo off

SET R="../bin/R#"
SET RUN="./mrm_quantify.R"
SET MRM="S:\2018\WYL\20191230_WYL_MRM\MRM.xlsx"

SET EXPORT1=S:\2018\WYL\20191230_WYL_MRM\MeiZhou\result
SET DATA1=S:\2018\WYL\20191230_WYL_MRM\MeiZhou\raw

%R% %RUN% --Cal "%DATA1%\20191224cal" --data "%DATA1%\20191224sample" --MRM %MRM% --export "%EXPORT1%\20191224sample"

EXIT 0

%R% %RUN% --Cal "%DATA1%\20191225cal" --data "%DATA1%\20191225sample" --MRM %MRM% --export "%EXPORT1%\20191225sample"
%R% %RUN% --Cal "%DATA1%\20191223cal" --data "%DATA1%\20191223sample" --MRM %MRM% --export "%EXPORT1%\20191223sample"


SET EXPORT2=S:\2018\WYL\20191230_WYL_MRM\XBL\result
SET DATA2=S:\2018\WYL\20191230_WYL_MRM\XBL\raw

%R% %RUN% --Cal "%DATA2%\20191217cal" --data "%DATA2%\20191217sample(UA)-1" --MRM %MRM% --export "%EXPORT2%\20191217sample(UA)-1"
%R% %RUN% --Cal "%DATA2%\20191218cal" --data "%DATA2%\20191218sample(UA+NOCA)" --MRM %MRM% --export "%EXPORT2%\20191218sample(UA+NOCA)"
%R% %RUN% --Cal "%DATA2%\20191219cal" --data "%DATA2%\20191219sample" --MRM %MRM% --export "%EXPORT2%\20191219sample"
%R% %RUN% --Cal "%DATA2%\20191220cal" --data "%DATA2%\20191220sample" --MRM %MRM% --export "%EXPORT2%\20191220sample"
%R% %RUN% --Cal "%DATA2%\20191223cal" --data "%DATA2%\20191223sample" --MRM %MRM% --export "%EXPORT2%\20191223sample"