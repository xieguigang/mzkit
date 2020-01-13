@echo off

SET R="../bin/R#.exe"
SET raw=../../DATA/test/003_Ex2_Orbitrap_CID

%R% ./mzXML.mgf.R --mzXML "%raw%.mzXML" --and.ms1 --out "%raw%.includes_ms1.mgf"