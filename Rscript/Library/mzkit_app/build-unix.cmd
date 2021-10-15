@echo off

SET drive=%~d0
SET Rscript="%drive%\GCModeller\src\R-sharp\App\net5.0\Rscript.exe"

%Rscript% --build

pause