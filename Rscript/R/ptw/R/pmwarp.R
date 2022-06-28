pmwarp <- function (ref, samp, optim.crit, init.coef, try = FALSE,
                    mode = c("forward", "backward"),
                    trwdth, trwdth.res, smooth.param, ...)
{
  mode <- match.arg(mode)
  
  ## Multiply coefficients to prevent them from becoming too 
  ## small for numeric precision.
  n <- length(init.coef)
  ncr <- ncol(ref)
  time <- (1:ncr) / ncr
  B <- matrix(time, nrow = ncr, ncol = n)
  B <- t(apply(B, 1, cumprod))/B
  a <- init.coef * ncr^(0:(n-1))

  if (optim.crit == "RMS" & smooth.param > 0) {
    samp.sm <- t(apply(samp, 1, difsm, smooth.param))
    ref.sm <- t(apply(ref, 1, difsm, smooth.param))
  }
  
  if (!try) { # perform optimization
    switch(optim.crit,
           RMS = {
             if (smooth.param > 0) {
               Opt <- optim(a, RMS, NULL, ref.sm, samp.sm, B, mode = mode, ...)
             } else {
               Opt <- optim(a, RMS, NULL, ref, samp, B, mode = mode, ...)
             }},
           WCC = {
             wghts <- 1 - (0:trwdth)/trwdth
             ref.acors <- apply(ref, 1, wac, trwdth = trwdth, wghts = wghts)
             Opt <- optim(a, WCC, NULL, ref, samp, B,
                          trwdth = trwdth, wghts = wghts,
                          ref.acors = ref.acors, mode = mode, ...)
           })
    
    a <- c(Opt$par)
    v <- Opt$value

    ## if the optimization is done with a different smoothing or a
    ## different triangle, the final value for the optimization
    ## criterion is recalculated using the "original" data
    if ((optim.crit == "RMS" && smooth.param > 0) ||
        (optim.crit == "WCC" && trwdth != trwdth.res)) {
      v <- switch(optim.crit,
                  RMS = RMS(a, ref, samp, B, mode = mode),
                  WCC = WCC(a, ref, samp, B, trwdth.res, mode = mode))
    }
  }

  ## calculate, or possibly re-calculate, quality of current solution
  if (try) {
    if (optim.crit == "WCC") {
      v <- WCC(a, ref, samp, B, trwdth.res, mode = mode)
    } else {
      if (smooth.param > 0) {
        v <- RMS(a, ref.sm, samp.sm, B, mode = mode)
      } else {      
        v <- RMS(a, ref, samp, B, mode = mode)
      }
    }
  }

  ## back-transform coefficients
  w <- B %*% a
  a <- a/ncr^(0:(n-1))
  
  list(w = w, a = a, v = v)
} 
