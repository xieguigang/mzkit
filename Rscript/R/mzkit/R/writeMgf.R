#Region "Microsoft.ROpen::54175876760b25fe784751f40a477758, mgf.R"

# Summaries:

# write.mgf <- function(meta, ms2, path, verbose = TRUE) {...
# mgf.ion <- function(mz, rt, ms2, charge = "1", title = "Unknown name ion", ms1.into = 100) {...
# read.mgf <- function(fileName) {...
# parse.mgf <- function(buffer) {...
# parse.mgf.title_meta <- function(title) {...

#End Region

#' Write ms2 data as a mgf spectrum data file.
#'
#' @param meta A \code{dataframe} object that contains metainfo annotation data,
#'    this \code{dataframe} should contains at least:
#'    \code{mz}, \code{rt}, \code{charge}, \code{libname} value fields.
#' @param ms2 Ms2 library data. A list of \code{dataframe} object, where the list
#'    member name is the \code{libname} value in \code{meta} parameter value.
#'    Or this parameter can be a function to produce such data by a given libname
#'    value.
#' @param path The file location of where the generated mgf file was saved.
#'
write.mgf <- function(meta, ms2, path, verbose = TRUE) {

  mz       <- meta[, "mz"]      %=>% as.numeric;
  rt       <- meta[, "rt"]      %=>% as.numeric;
  charge   <- meta[, "charge"]  %=>% as.numeric;
  libnames <- meta[, "libname"] %=>% as.character;

  write <- path %=>% File.Open;

  if (is.function(ms2)) {
    lib_ms2 = ms2;
  } else {
    lib_ms2 = function(libname) ms2[[libname]];
  }

  for(i in 1:nrow(meta)) {
    if (verbose) {
      cat(libnames[i]);
      cat(" ");
    }

    spectra <- lib_ms2(libnames[i]);
    ion     <- mgf.ion(
      mz[i], rt[i],
      spectra,
      charge[i],
      libnames[i]
    );

    ion %=>% write;

    if (verbose) {
      cat("...OK!\n");
    }
  }

  invisible(NULL);
}

#' Generate mgf ion from data.
#'
#' @param ms2 This ms2 matrix object can be in two formats:
#' \enumerate{
#' \item \code{list} ms2 data is consist with element sequence, each element must have property \code{mz} and \code{into}.
#' \item \code{data.frame} The data frame object should have \code{mz} and \code{into} column.
#' }
mgf.ion <- function(mz, rt, ms2, charge = "1", title = "Unknown name ion", ms1.into = 100) {
  lines    <- c();
  lines[1] <- "BEGIN IONS";
  lines[2] <- sprintf("PEPMASS=%s %s", mz, ms1.into);
  lines[3] <- sprintf("TITLE=%s", title);
  lines[4] <- sprintf("RTINSECONDS=%s", rt);
  lines[5] <- sprintf("CHARGE=%s%s", charge, if(charge>0) "+" else "-") ;

  ms2_type   <- ms2 %=>% GetType;
  type_enums <- primitiveTypes();

  if (ms2_type == type_enums$list) {
    for(mz in ms2) {
      lines <- append(lines, sprintf("%s %s", mz[["mz"]], mz[["into"]]));
    }
  } else if (ms2_type == type_enums$data.frame) {
    lines <- append(lines, sprintf("%s %s", ms2[, "mz"], ms2[, "into"]));
  } else {
    stop("The ms2 data should be a list or a data.frame");
  }

  append(lines, "END IONS");
}
