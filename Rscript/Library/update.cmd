@echo off

SET js_url="https://mzkit.org/assets/js/R_syntax.js"
SET Rscript="\GCModeller\src\R-sharp\App\net8.0\Rscript.exe"
SET R="\GCModeller\src\R-sharp\App\net8.0\R#.exe"

%Rscript% --build /src ./MSI_app/   /save ./mzkit.zip     --skip-src-build --github-page %js_url%
%Rscript% --build /src ./mzkit_app/ /save ./msImaging.zip --skip-src-build --github-page %js_url%

%R% --install.packages ./mzkit.zip 
%R% --install.packages ./msImaging.zip