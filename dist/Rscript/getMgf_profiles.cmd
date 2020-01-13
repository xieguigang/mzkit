@echo off

SET R="../bin/R#.exe"
SET raw=D:\web\QC4

%R% ./mzXML.mgf.R --mzXML "%raw%.mzXML" --out "%raw%.profiles.mgf"