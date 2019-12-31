# Hello, world!
#
# This is an example function named 'hello' 
# which prints 'Hello, world!'.
#
# You can learn more about package authoring with RStudio at:
#
#   http://r-pkgs.had.co.nz/
#
# Some useful keyboard shortcuts for package authoring:
#
#   Build and Reload Package:  'Ctrl + Shift + B'
#   Check Package:             'Ctrl + Shift + E'
#   Test Package:              'Ctrl + Shift + T'

Enumerate.Formula.mass <- function(mass, charges = NULL, tolerance = function(m1, m2) abs(m1 - m2) <= 0.3) {

}

Enumerate.Formula.MZ <- function(mz, tolerance = function(mz1, mz2) ppm(mz1 - mz2) <= 20) {

}