imports "assembly" from "mzkit";
imports "math" from "mzkit";

#' Hello, world!
#'
#' This is an example function named 'hello' 
#' which prints 'Hello, world!'.
#'
#' You can learn more about package authoring with RStudio at:
#'
#'   http://r-pkgs.had.co.nz/
#'
#' Some useful keyboard shortcuts for package authoring:
#'
#'   Install Package:           'Ctrl + Shift + B'
#'   Check Package:             'Ctrl + Shift + E'
#'   Test Package:              'Ctrl + Shift + T'
#' 
let .onLoad as function() {
  cat("\n");

  print(description(package = "mzkit")$title);
  print(description(package = "mzkit")$description);
  
  cat("\n");
  
  print("Visit of mzkit HOME:");
  print("https://mzkit.org/");
  
  cat("\n");
}
