#!/bin/bash

# start the httpd web server
# listen on port 80
# using /@set parameter to initialize the mysql connection in visitor statics plugin in dll file: VisitStat.dll
./bin/httpd.exe /start /port 80 /@set "host=localhost;mysql_port=3306;user=root;password=1234;database=test"