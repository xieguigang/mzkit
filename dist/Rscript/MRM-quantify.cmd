@echo off

SET R="bin/R#.exe"

SET CAL=""
SET data=""
SET MRM=""
SET output=""

CALL %R% --Cal %CAL% --data %data% --MRM %MRM% --export %output%