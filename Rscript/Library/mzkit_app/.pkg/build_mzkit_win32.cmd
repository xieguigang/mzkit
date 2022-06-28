@echo off

SET mzkit_renv=../../../../dist/bin
SET Rscript="%mzkit_renv%/Rscript.exe"
SET REnv="%mzkit_renv%/R#.exe"

SET mzkit_pkg=../../../../src/mzkit/setup/mzkit.zip

%Rscript% --build /src ../ /save %mzkit_pkg%
%REnv% --install.packages %mzkit_pkg%

pause