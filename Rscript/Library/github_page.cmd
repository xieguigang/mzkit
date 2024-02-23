@echo off

SET js_url="https://mzkit.org/assets/js/R_syntax.js"
SET Rscript="\GCModeller\src\R-sharp\App\net6.0\Rscript.exe"

%Rscript% --build /src ./MSI_app/   --skip-src-build --github-page %js_url%
%Rscript% --build /src ./mzkit_app/ --skip-src-build --github-page %js_url%