@echo off

SET Rscript="../../../dist/bin/Rscript.exe"

%Rscript% --build /save ../../../src/mzkit/setup/mzkit.zip

pause