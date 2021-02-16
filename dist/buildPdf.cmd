@echo off

SET wkhtmltopdf="../src/mzkit/dist/wkhtmltopdf.exe"

%wkhtmltopdf% --javascript-delay 1000 --margin-bottom 5 --margin-top 5 --margin-left 5 --margin-right 5 https://mzkit.org/ "./README.pdf"