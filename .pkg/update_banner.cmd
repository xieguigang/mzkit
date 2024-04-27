@echo off

SET R_proj=/GCModeller/src/R-sharp
SET R_HOME=%R_proj%/App/net6.0
SET REnv="%R_HOME%/R#.exe"
SET updater=%R_proj%/studio/code_banner.R

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../Rscript

git add -A
git commit -m "update source file banner headers![Rscript api package module]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/assembly

git add -A
git commit -m "update source file banner headers![MS assembly file]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/metadb

git add -A
git commit -m "update source file banner headers![metabolite annotatin library]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/metadna

git add -A
git commit -m "update source file banner headers![metadna]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/mzkit

git add -A
git commit -m "update source file banner headers![mzkit workbench desktop application]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/mzmath

git add -A
git commit -m "update source file banner headers![MS math algorithm]"

%REnv% %updater% --banner-xml ../MIT.xml --proj-folder ../src/visualize

git add -A
git commit -m "update source file banner headers![MS data plot]"

pause