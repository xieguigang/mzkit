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

#' Read a given mgf file
#'
#' @param fileName A single mgf data file its file path.
#'
#' @return Returns a list of mgf ions that parsed from the given mgf spectrum data file.
#'
read.mgf <- function(fileName) {
  lines  <- fileName %=>% ReadAllLines;
  ions   <- list();
  buffer <- c();
  index  <- 1;

  for(i in 1:length(lines)) {
    line   <- lines[i];
    buffer <- append(buffer, line);

    if (line == "END IONS") {
      ions[[index]] <- buffer %=>% parse.mgf;
      buffer        <- c();
      index         <- index + 1;
    }
  }

  if (length(buffer) > 0) {
    ions[[index]] <- buffer %=>% parse.mgf;
  }

  names(ions) <- sapply(ions, function(i) i$title);
  ions;
}

#' Parse mgf ion data from text buffer.
#'
parse.mgf <- function(buffer) {

  # The first line of the buffer is BEGIN IONS
  # so start from the second line
  i    <- 2;
  meta <- list();

  while(TRUE) {
    p <- InStr(buffer[i], "=");

    if (p > 0) {
      name  <- substr(buffer[i], 1, p - 1);
      value <- substring(buffer[i], p + 1);
      meta[[name]] <- value;

      i <- i + 1;
    } else {
      break;
    }
  }

  mz   <- c();
  into <- c();

  # The last line of the buffer is END IONS
  # so end before reach the last line
  for (j in i:(length(buffer) - 1)) {
    tokens <- strsplit(buffer[j] %=>% Trim, "\\s+")[[1]];

    if (length(tokens) != 2) {
      stop("Incorrect file format!");
    } else {
      mz   <- append(mz, tokens[1]);
      into <- append(into, tokens[2]);
    }
  }

  # 20191211 The mz and into parse from mgf file is string
  # will change to factor by data.frame
  # change mode to numeric at first to avoid such bug problem in R language.
  ms2 <- data.frame(mz = as.numeric(mz), into = as.numeric(into));
  mz  <- strsplit(meta[["PEPMASS"]], "\\s+")[[1]];
  title <- parse.mgf.title_meta(meta[["TITLE"]]);

  list(mz1      = mz[1]                 %=>% as.numeric,
       ms1.into = mz[2]                 %=>% as.numeric,
       rt       = meta[["RTINSECONDS"]] %=>% as.numeric,
       title    = title$title_string,
       meta     = title$meta,
       charge   = meta[["CHARGE"]],
       ms2      = ms2
  );
}

#' Parse meta data contains in title
#'
#' @param title The title string text
#'
parse.mgf.title_meta <- function(title) {
  title_string <- Strings.Split(title, " ")[1];
  title_meta <- Strings.Replace(title, title_string, "") %=>% Trim;

  if (Strings.Len(title_meta) == 0) {
    meta <- list();
  } else {
    title_meta <- Strings.Split(title_meta, '", ');
    title_meta <- GetTagValue(title_meta, ':"');

    meta <- list();

    for(attr in title_meta) {
      meta[[attr$name]] <- Strings.Trim(attr$value, "\"");
    }
  }

  list(title_string = title_string, meta = meta);
}
