#Region "Microsoft.ROpen::601acd34f7346ce56eb5ea91ee50432d, R\readMgf.R"

    # Summaries:

    # read.mgf <- function(fileName) {...
    # parse.mgf <- function(buffer) {...
    # parse.mgf.title_meta <- function(title) {...

#End Region

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
