@echo

SET debug_port=33361

mzkit_win32 --debug --port=%debug_port%
Rscript Rstudio/Pipeline/ServiceHub/MSI-host.R --debug=%debug_port%