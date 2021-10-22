@echo off

SET root=%CD%
SET base=%CD%/../

SET MSI_app=%base%/Rscript/Library/MSI_app
SET jump=run

goto :%jump%

REM ----===== git sync function =====----
:sync_git
SETLOCAL
SET _repo=%1

cd "%_repo%"

echo "git repository directory:"
echo " --> %CD%"

cd "%_repo%/.github"
CALL sync_multiplegit.cmd
cd %base%

:echo
:echo
echo "sync of git repository %_repo% job done!"
echo "---------------------------------------------------------"
:echo
:echo

ENDLOCAL & SET _result=0
goto :%jump%

REM ----===== end of function =====----

:run

SET jump=msi_app
CALL :sync_git %MSI_app%
:msi_app

cd %root%
CALL sync_multiplegit.cmd

pause
exit 0