@echo off

SET drive=%~d0
SET R_HOME=%drive%\GCModeller\src\R-sharp\App\net6.0
SET Rscript="%R_HOME%/Rscript.exe"
SET REnv="%R_HOME%/R#.exe"
SET js_url="https://mzkit.org/assets/js/R_syntax.js"

%Rscript% --build /src ../ /save ./mzkit.zip --skip-src-build  --github-page %js_url%
%REnv% --install.packages ./mzkit.zip

pause