@echo

SET debug_port=33361
SET localhost="Rstudio/Pipeline/ServiceHub/MSI-host.R"

START mzkit_win32 --debug --port=%debug_port%
START Rscript %localhost% --debug=%debug_port%