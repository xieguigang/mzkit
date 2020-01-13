@echo off

SET R="../bin/R#.exe"
SET raw=S:\艾苏莱\全靶20191205\Research\QE\pos\2mz\QC1-2

%R% ./mzXML.mgf.R --mzXML "%raw%.mzXML" --and.ms1 --out "%raw%.profiles.mgf"