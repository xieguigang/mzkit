#Region "Microsoft.ROpen::65e6df594237eed3012943702a3930bb, asciiView.R"

    # Summaries:

    # asciiView <- function(spectra,into.cutoff = 0.05,maxWidth = 80,show.mz.labels = TRUE) {...
    # lenOf <- function(x) {if (x < 10) {...

#End Region

#' ASCII View of Spectra Matrix
#'
#' @param spectra The spectra matrix should contains at least two column:
#'   first column for \code{m/z} and second column for \code{intensity}.
#' @param into.cutoff All of the \code{m/z} fragment which its intensity
#'   value less than this percetange cutoff, will be display on the output
#' @param maxWidth The max width of the text viewer in ncharacters.
#'
#' @return The text of the spectra viewer result, you should use \code{cat}
#'   function for display this \code{spectra} on the console
#'
asciiView <- function(spectra,
                      into.cutoff = 0.05,
                      maxWidth = 80,
                      show.mz.labels = TRUE) {

  # Extract spectra matrix data
  mz <- spectra[, 1] %=>% as.numeric;
  # column steps in viewer
  delta.mz <- (max(mz) - min(mz)) / (maxWidth - 10);
  # cleanup matrix
  into <- spectra[, 2] %=>% as.numeric;
  into <- into / max(into);
  mz <- mz[into >= into.cutoff];
  into <- into[into >= into.cutoff];

  offset <- 6;
  fill.1 <- rep(" ", times = offset);
  fill.2 <- rep(" ", times = offset);
  fill.3 <- rep(" ", times = offset);
  fill.4 <- rep(" ", times = offset);
  fill.5 <- rep(" ", times = offset);
  fill.axis <- rep("-", times = offset);
  labels <- list();

  mzx <- min(mz);
  mzl <- 0;

  for(i in offset:maxWidth) {
    mzi <- (mz > mzl) & (mz <= mzx);
    mzl <- mzx;
    mzx <- mzx + delta.mz;

    if (sum(mzi) == 0) {
      # no fragment;
      # fill empty
      fill.1 <- append(fill.1, " ");
      fill.2 <- append(fill.2, " ");
      fill.3 <- append(fill.3, " ");
      fill.4 <- append(fill.4, " ");
      fill.5 <- append(fill.5, " ");
      fill.axis <- append(fill.axis, "-");
    } else {
      int <- max(into[mzi]);
      mz.i <- mz[int == into];
      fill.axis <- append(fill.axis, "+");
      labels[[as.character(i)]] <- mz.i;

      if (int >= 0.85) {
        # fill 123
        fill.1 <- append(fill.1, "|");
        fill.2 <- append(fill.2, "|");
        fill.3 <- append(fill.3, "|");
        fill.4 <- append(fill.4, "|");
        fill.5 <- append(fill.5, "|");
      } else if (int >= 0.6) {
        # fill 23
        fill.1 <- append(fill.1, " ");
        fill.2 <- append(fill.2, "|");
        fill.3 <- append(fill.3, "|");
        fill.4 <- append(fill.4, "|");
        fill.5 <- append(fill.5, "|");
      } else if (int >= 0.4) {
        # fill 23
        fill.1 <- append(fill.1, " ");
        fill.2 <- append(fill.2, " ");
        fill.3 <- append(fill.3, "|");
        fill.4 <- append(fill.4, "|");
        fill.5 <- append(fill.5, "|");
      } else if (int >= 0.2) {
        # fill 23
        fill.1 <- append(fill.1, " ");
        fill.2 <- append(fill.2, " ");
        fill.3 <- append(fill.3, " ");
        fill.4 <- append(fill.4, "|");
        fill.5 <- append(fill.5, "|");
      } else {
        # fill 3
        fill.1 <- append(fill.1, " ");
        fill.2 <- append(fill.2, " ");
        fill.3 <- append(fill.3, " ");
        fill.4 <- append(fill.4, " ");
        fill.5 <- append(fill.5, "|");
      }
    }
  }

  a <- paste(fill.1, collapse = "");
  b <- paste(fill.2, collapse = "");
  c <- paste(fill.3, collapse = "");
  d <- paste(fill.4, collapse = "");
  e <- paste(fill.5, collapse = "");
  f <- paste(fill.axis, collapse = "");

  views <- c("", a, b, c, d, e, f);
  lenOf <- function(x) {
    if (x < 10) {
      1;
    } else if (x < 100) {
      2;
    } else if (x < 1000) {
      3;
    } else if (x < 10000) {
      4;
    }
  }

  if (show.mz.labels) {
    first <- "m/z:  ";
    label_matrix <- list();

    for(i in offset:maxWidth) {
      if (is.null(labels[[as.character(i)]])) {
        first <- append(first, " ");
      } else {
        first <- append(first, "|");
      }
    }

    views <- append(views, paste(first, collapse = ""));
    d <- 2;
    index <- names(labels)

    for(j in index) {
      mz <- labels[[j]];
      row <- round(mz) %=>% as.character;
      n <- lenOf(mz) + d;
      row <- append(rep(" ", times = offset - n), row);
      row <- append(row, rep(" ", times = d));
      labels[[j]] <- NULL;

      j <- as.numeric(j);

      for (i in offset:maxWidth) {
        if (i == j) {
          row <- append(row, "+");
        } else if (i < j) {
          row <- append(row, "-");
        } else if (is.null(labels[[as.character(i)]])) {
          row <- append(row, " ");
        } else {
          row <- append(row, "|");
        }
      }

      views <- append(views, paste(row, collapse = ""));
    }
  }

  paste(append(views, ""), collapse = "\n");
}
