#Region "Microsoft.ROpen::7c6a55fb86193f1bb8f2f7f1152688ad, MolWeight.R"

# Summaries:

# MolWeight <- function() {...
# .Weight <- function(symbol) {...
# .Eval <- function(formula) {if ((tokens = Strings.Split(formula, ""))[1] %in% ionSymbols) {...
# Mul <- function(token) {...

#End Region

#' Molecule weight calculate helper
#'
#' @return A list module that contains member function:
#'    \enumerate{
#'    \item \code{Eval}
#'    \item \code{Weight}
#' }
MolWeight <- function() {

  ionSymbols <- c("+", "-");
  weights    <- list(
    # M is the target molecule, do not calculate!
    "M"       = 0,
    "H"       = 1.007276,
    "Na"      = 22.98976928,
    "NH4"     = 18.035534,
    "K"       = 39.0983,
    "H2O"     = 18.01471,
    "ACN"     = 41.04746,  # Acetonitrile (CH3CN)
    "CH3OH"   = 32.03773,
    "DMSO"    = 78.12089,  # dimethyl sulfoxide (CH3)2SO
    "IsoProp" = 60.058064, # Unknown
    "Cl"      = 35.446,
    "FA"      = 46.00548,  # Unknown
    "Hac"     = 60.04636,  # AceticAcid (CH3COOH)
    "Br"      = 79.901,
    "TFA"     = 113.9929   # Unknown
  );

  .Weight <- function(symbol) {
    w <- weights[[symbol]];

    if (!IsNothing(w)) {
      w;
    } else {
      msg <- "Symbol '%s' is not exists in table!";
      msg <- sprintf(msg, symbol);
      warning(msg);
      -1;
    }
  }

  .Eval <- function(formula) {
    if ((tokens = Strings.Split(formula, ""))[1] %in% ionSymbols) {
      formula <- sprintf("0H%s", formula);
    }

    mt      <- strsplit(formula, "[+-]")[[1]];
    op      <- tokens[tokens %in% ionSymbols];
    x       <- 0;
    next_op <- "+";

    for(i in 1:length(mt)) {
      token <- Mul(mt[i]);
      M     <- token[["M"]];
      token <- token[["name"]];
      w     <- .Weight(token);

      # due to the reason of M symbol equals to ZERO
      # And -1 for not found
      # So change the assert expression <= 0 to < 0
      if (w < 0) {
        msg <- "Unknown symbol in: '%s', where symbol=%s";
        msg <- sprintf(msg, formula, token);
        stop(msg);
      }

      if (next_op == "+") {
        x <- x + (M * w);
      } else {
        x <- x - (M * w);
      }

      if (!IsNothing(op) && (op != FALSE) && (i <= length(op))) {
        next_op <- op[i];
      }
    }

    x;
  }

  # ASCII code of [0, 9], using for determine that token parts is
  # a numeric token or not.
  x0 <- utf8ToInt("09");
  x9 <- x0[2];
  x0 <- x0[1];

  Mul <- function(token) {
    n   <- c();
    len <- Strings.Len(token);
    xt  <- utf8ToInt(token);

    for (i in 1:len) {
      x <- xt[i];

      if (x >= x0 && x <= x9) {
        n <- append(n, xt[i]);
      } else {
        break;
      }
    }

    if (length(n) == 0) {
      name <- token;
      M    <- 1;
    } else {
      # substr(x, start, stop)
      token <- substr(token, length(n)+1, length(xt));
      name  <- token;
      M     <- Strings.Join(Chr(n), "") %=>% as.numeric;
    }

    list(name = name, M = M);
  }

  list(Weight = .Weight,
       weights = weights,
       Eval   = .Eval
  );
}
