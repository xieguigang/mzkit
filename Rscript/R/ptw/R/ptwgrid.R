ptwgrid <- function(ref, samp, selected.traces,
                    coef.mins, coef.maxs, coef.lengths,
                    optim.crit = c("WCC", "RMS"),
                    smooth.param = 1e05,
                    trwdth = 20) {
  optim.crit <- match.arg(optim.crit)

  if (is.vector(ref)) ref <- matrix(ref, nrow = 1)
  if (is.vector(samp)) samp <- matrix(samp, nrow = 1)
  if (nrow(ref) > 1 && nrow(ref) != nrow(samp)) {
    stop("The number of references does not equal the number of samples")
  }

  if (!missing(selected.traces)) {
    samp <- samp[selected.traces, , drop = FALSE]
    if (nrow(ref) > 1) {
      ref <- ref[selected.traces, , drop = FALSE]
    }
  }

  if (optim.crit == "WCC" && nrow(ref) != nrow(samp)) {
    ref <- matrix(ref,
      nrow = nrow(samp),
      ncol = ncol(ref), byrow = TRUE
    )
  }

  if (is.vector(coef.mins)) coef.mins <- matrix(coef.mins, nrow = 1)
  if (is.vector(coef.maxs)) coef.maxs <- matrix(coef.maxs, nrow = 1)
  if (is.vector(coef.lengths)) coef.lengths <- matrix(coef.lengths, nrow = 1)

  n <- length(coef.mins)
  ncr <- ncol(ref)
  time <- (1:ncr) / ncr
  B <- matrix(time, nrow = ncr, ncol = n)
  B <- t(apply(B, 1, cumprod)) / B

  # Make matrix with all combinations of coefficients on the grid.
  coef.list <- mapply(
    FUN = function(x, y, z) {
      seq(x, y, length.out = z)
    },
    t(coef.mins), t(coef.maxs), t(coef.lengths), SIMPLIFY = FALSE
  )
  coef.combs <- as.matrix(do.call(expand.grid, coef.list))

  # Calculate RMS or WCC values on the grid.
  switch(optim.crit,
    RMS = {
      if (smooth.param > 0) {
        samp.sm <- t(apply(samp, 1, difsm, smooth.param))
        ref.sm <- t(apply(ref, 1, difsm, smooth.param))
        G <- apply(
          t(t(coef.combs) * ncr^(0:(n - 1))), 1, RMS, ref.sm,
          samp.sm, B
        )
      } else {
        G <- apply(t(t(coef.combs) * ncr^(0:(n - 1))), 1, RMS, ref, samp, B)
      }
    },
    WCC = {
      wghts <- 1 - (0:trwdth) / trwdth
      ref.acors <- apply(ref, 1, wac,
        trwdth = trwdth,
        wghts = wghts
      )
      G <- apply(t(t(coef.combs) * ncr^(0:(n - 1))), 1, WCC, ref, samp, B,
        trwdth = trwdth, wghts = wghts, ref.acors = ref.acors
      )
    }
  )

  # Make array, calculate indices into array corresponding to coefficient
  # combinations, put RMS or WCC values in array.
  A <- array(data = NA, dim = coef.lengths)
  inds.list <- lapply(coef.lengths, function(x) {
    1:x
  })
  inds <- as.matrix(do.call(expand.grid, inds.list))
  A[inds] <- G
  A
}