@echo off

REM install mysql to vb.net source code into the data project

REM install mysql ORM adapter
SET dir="./mysql"
reflector --reflects /sql ./data-center.sql /namespace "mysql" /split /auto_increment.disable
RD /S /Q %dir%
mkdir %dir%
xcopy "./data-center/*.*" %dir% /s /h /d /y /e /f /i
RD /S /Q "./data-center/"

REM generates the mysql development sdk documents
reflector /MySQL.Markdown /sql ./data-center.sql > ./dev.md