@echo off

@echo off

SET R="../bin/R#"
SET RUN="./mzML_chromatogramPlots.R"
SET MRM="S:\2018\WYL\20191230_WYL_MRM\MRM.xlsx"

SET DATA1=S:\2018\WYL\20191230_WYL_MRM\MeiZhou\raw
SET OUT1=S:\2018\WYL\20191230_WYL_MRM\MeiZhou\plots

%R% %RUN% --MRM %MRM% --mzML "%DATA1%/201912wash" --output "%OUT1%/201912wash"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191223cal" --output "%OUT1%/20191223cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191223sample" --output "%OUT1%/20191223sample"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191224cal" --output "%OUT1%/20191224cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191224sample" --output "%OUT1%/20191224sample"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191225cal" --output "%OUT1%/20191225cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA1%/20191225sample" --output "%OUT1%/20191225sample"

SET DATA2=S:\2018\WYL\20191230_WYL_MRM\XBL\raw
SET OUT2=S:\2018\WYL\20191230_WYL_MRM\XBL\plots

%R% %RUN% --MRM %MRM% --mzML "%DATA2%/201912wash" --output "%OUT2%/201912wash"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191217cal" --output "%OUT2%/20191217cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191217sample(UA)-1" --output "%OUT2%/20191217sample(UA)-1"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191218cal" --output "%OUT2%/20191218cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191218sample(UA+NOCA)" --output "%OUT2%/20191218sample(UA+NOCA)"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191219cal" --output "%OUT2%/20191219cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191219sample" --output "%OUT2%/20191219sample"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191220cal" --output "%OUT2%/20191220cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191220sample" --output "%OUT2%/20191220sample"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191223cal" --output "%OUT2%/20191223cal"
%R% %RUN% --MRM %MRM% --mzML "%DATA2%/20191223sample" --output "%OUT2%/20191223sample"