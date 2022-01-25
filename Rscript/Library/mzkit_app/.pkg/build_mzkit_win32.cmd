@echo off

SET Rscript="../../../../dist/bin/Rscript.exe"

%Rscript% --build /src ../ /save ../../../../src/mzkit/setup/mzkit.zip

pause