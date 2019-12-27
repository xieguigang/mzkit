@echo off

REM example script for running ProteoWizard.d worker server
REM
REM The daemon server required two parameters:
REM
REM bin: The windows exe file path of the ProteoWizard commandline tool
REM oss: The mount point of the external file system.
REM
REM All of these two parameter is the file path location, and should without any whitespace in the path.

"App/httpd" /start /port 81 /@set "bin='C:\Program Files\ProteoWizard\ProteoWizard 3.0.10650\msconvert.exe';oss=D:\ProteoWizard.d\mount_237"
