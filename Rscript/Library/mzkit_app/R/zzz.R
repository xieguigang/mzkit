imports "assembly" from "mzkit";
imports "math" from "mzkit";

require(GCModeller);

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
const .onLoad = function() {
  cat("\n");

  print(description(package = "mzkit")$title);
  print(description(package = "mzkit")$description);
  
  cat("\n");
  
  print("Visit of mzkit HOME:");
  print("https://mzkit.org/");
  
  cat("\n\n");
  cat("MZKit is an open source raw data file toolkit for mass spectrometry\n");
  cat("data analysis, provides by the BioNovoGene corporation.\n");
  cat("\n");
  cat("The features of mzkit inlcudes: raw data file content viewer\n");
  cat("(XIC/TIC/Mass spectral plot/MS-Imaging), build molecule network,\n");
  cat("formula de-novo search, de-novo annotation of the unknown\n");
  cat("metabolite features, MALDI single cell metabolomics data analysis,\n");
  cat("pathological slide viewer and targeted data quantification.\n");

  cat("\n");
}
