bestref <- function(x, optim.crit = c("WCC", "RMS"),
                    trwdth = 20, wghts = NULL, smooth.param = 0) {
  optim.crit <- match.arg(optim.crit)

  if (inherits(x, "array") & length(dim(x)) == 3) {
    nsamp <- dim(x)[1]

    xl <- vector("list", nsamp)

    for (i in 1:nsamp) {
      if (smooth.param > 0) {
        xl[[i]] <- apply(x[i, , ], 2, difsm, smooth.param)
      } else {
        xl[[i]] <- x[i, , ]
      }
    }

    xl <- lapply(xl, t)
    best <- numeric()
    best.mat <- matrix(NA, nrow = dim(x)[3], ncol = nsamp)

    for (i in 1:length(xl)) {
      calc <- bestref(xl[[i]],
        optim.crit = optim.crit, trwdth = trwdth,
        wghts = wghts, smooth.param = smooth.param
      )
      best[i] <- calc[[1]]
      best.mat[, i] <- calc[[2]]
    }

    list(best.ref = best, crit.values = best.mat)
  } else {
    if (optim.crit == "RMS") {
      if (smooth.param > 0) {
        x <- t(apply(x, 1, difsm, smooth.param))
      }

      x.dist <- as.matrix(dist(x))
      dimnames(x.dist) <- NULL

      list(
        best.ref = which.min(colSums(x.dist^2)),
        crit.values = colSums(x.dist^2)
      )
    } else {
      if (is.null(wghts)) {
        wghts <- 1 - (0:trwdth) / trwdth
      }

      nx <- nrow(x)
      x.wcc <- rep(0, nx * (nx - 1) / 2)

      counter <- 0
      for (i in 1:(nx - 1)) {
        for (j in (i + 1):nx) {
          counter <- counter + 1
          x.wcc[counter] <- wcc(x[i, ], x[j, ], trwdth, wghts)
        }
      }

      attr(x.wcc, "Size") <- nx
      class(x.wcc) <- "dist"
      x.wcc <- as.matrix(x.wcc)
      dimnames(x.wcc) <- NULL
      x.wcc <- x.wcc + t(x.wcc)

      list(
        best.ref = which.max(colSums(x.wcc)),
        crit.values = colSums(x.wcc)
      )
    }
  }
}