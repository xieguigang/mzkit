# wccStick.R

## access to the C functions in wccStick.c. Both pat1 and pat2 should
## be two-column matrices, with rt in the first column and I in the
## second. Returns the wcc value for these two stick spectra.
wcc.st <- function(pat1, pat2, trwidth, acors1 = NULL, acors2 = NULL) {
  WAC <- WCC <- 0
  np1 <- nrow(pat1)
  np2 <- nrow(pat2)
  p1 <- c(pat1)
  p2 <- c(pat2)

  if (is.null(acors1))
      acors1 <- .C("st_WAC",
            as.double(p1),
            as.integer(np1),
            as.double(trwidth),
            WAC = as.double(WAC),
            PACKAGE = "ptw")$WAC
  if (is.null(acors2))
      acors2 <- .C("st_WAC",
            as.double(p2),
            as.integer(np2),
            as.double(trwidth),
            WAC = as.double(WAC),
            PACKAGE = "ptw")$WAC
  res <- .C("st_WCC",
            as.double(p1),
            as.integer(np1),
            as.double(p2),
            as.integer(np2),
            as.double(trwidth),
            WCC = as.double(WCC),
            PACKAGE = "ptw")$WCC
  
  res / (acors1*acors2)
}

wac.st <- function(pat1, trwidth) {
  WAC <- 0
  np1 <- nrow(pat1)
  p1 <- c(pat1)
  
  .C("st_WAC",
     as.double(p1),
     as.integer(np1),
     as.double(trwidth),
     WAC = as.double(WAC),
     PACKAGE = "ptw")$WAC
}


## #######################################################################
## analogon to pmwarp.R: the actual optimization. Changes w.r.t. pmwarp:
## 1) only present for WCC, so all references to optim.crit are taken out
## 2) call optim using the STWCC function rather than WCC
## 3) trwdth and trwdth.res should be given in REAL TIME UNITS, and
##    not in terms of sampling points
stwarp <- function (ref, samp, init.coef, try = FALSE, trwdth, 
                    trwdth.res, nGlobal, ...) 
{
  ncr <- ceiling(max(sapply(ref,
                            function(x)
                              ifelse(nrow(x) > 0,
                                     max(x[,"rt"]),
                                     NA)), na.rm = TRUE))
  
  ## first we take out the mass info that is not used here
  ref <- lapply(ref,
                function(x) x[, c("rt", "I"), drop = FALSE])
  samp <- lapply(samp,
                 function(x) matrix(cbind(x[, "rt"]/ncr, x[, "I"]),
                                    ncol = 2,
                                    dimnames = list(NULL, c("rt", "I"))))

  n <- length(init.coef)
  a <- init.coef * ncr^(0:(n-1))
  if (!try) {
    ref.acors <- sapply(ref, wac.st, trwdth)
    if (nGlobal > 0) {
      ## do several runs with a global optimizer
      
### Following lines were removed because nloptr wasmoved to the R
### archive - support no longer guaranteed. 
      ## NLOpt <- lapply(1:nGlobal,
      ##                 function(ii)
      ##                 nloptr(x0 = a, eval_f = STWCC,
      ##                        lb = rep(-1e+05, n), ub = rep(1e+05, n),
      ##                        opts = list(algorithm = "NLOPT_GN_CRS2_LM",
      ##                            maxeval = 1e+05),
      ##                        refList = ref, sampList = samp, trwdth = trwdth, 
      ##                        ref.acors = ref.acors))
      ## wccs <- sapply(NLOpt, "[[", "objective")
      ## a <- NLOpt[[which.min(wccs)]]$solution
      GASol <- lapply(1:nGlobal,
                      function(ii)
                        DEoptim(STWCC,
                                lower = rep(-1e+05, n), upper = rep(1e+05, n),
                                refList = ref, sampList = samp,
                                trwdth = trwdth, ref.acors = ref.acors,
                                control = list(strategy = 2, itermax = 2000,
                                               VTR = 0, trace = FALSE,
                                               NP = 100)))
      wccs <- sapply(GASol, function(xxx) xxx$optim$bestval)
        
      a <- GASol[[which.min(wccs)]]$optim$bestmem
    }
    
    Opt <- optim(a, STWCC, NULL, ref, samp, trwdth = trwdth,
                 ref.acors = ref.acors, ...)
  }
      
  a <- c(Opt$par)

  if (!missing(trwdth.res)) {
    ref.acors <- sapply(ref, wac.st, trwdth.res)
    v <- STWCC(a, ref, samp, trwdth.res, ref.acors)
  } else {
    v <- Opt$value
  }

  ## backtransform the coefficients...
  a <- a/ncr^(0:(n - 1))

  list(a = a, v = v)
}

STWCC <- function(warp.coef, refList, sampList, trwdth, ref.acors) {
  nmz <- length(refList)
  if (length(sampList) != nmz)
      stop("unequal mz lists in STWCC!")

  ## if a peak is absent in the reference, the corresponding value in
  ## ref.acors equals zero. So we can loop over all cases where this
  ## is the case - further speedup. Give a warning if too few peaks
  ## are found. For the moment, let's say less than 10%
  peaks.found <- which(ref.acors > 0)
  wccs <- sapply(peaks.found,
                 ## empty slots can also occur in the sample...
                 function(ii) {
                   if (nrow(sampList[[ii]]) == 0) {
                     NA
                   } else {
                     sampList[[ii]][,"rt"] <-
                       warp.time(sampList[[ii]][,"rt"], warp.coef)
                     
                     wcc.st(refList[[ii]],
                            sampList[[ii]],
                            trwidth = trwdth,
                            acors1 = ref.acors[ii])
                   }
                 })

  if (sum(!is.na(wccs)) < 0.1*nmz)
    warning("Peaks found in less than 10% of all mass bins in the reference...")
  

  1 - mean(wccs, na.rm = TRUE)
}

warp.time <- function(tp, coef) {
  powers <- 1:length(coef) - 1
  c(outer(tp, powers, FUN = "^") %*% coef)
}

## stick version of ptw: always global alignment, always using WCC, no
## selected traces,  no try argument. Here ref and sample are derived
## from peak tables as generated, e.g., by xcms, separated out into
## different mass channels - each channel is a list element. Function
## pktab2mzchannel is doing this.
stptw <- function (ref, samp, 
                   init.coef = c(0, 1, 0), 
                   trwdth = 20, trwdth.res = trwdth,
                   nGlobal = ifelse(length(init.coef) > 3, 5, 0),
                   ... )
{
  WCC <- stwarp(ref, samp, init.coef,
                trwdth = trwdth, trwdth.res = trwdth.res,
                nGlobal = nGlobal, ...)

  warped.sample <- lapply(samp,
                          function(x) {
                            if (nrow(x) > 0)
                              x[,"rt"] <- warp.time(x[, "rt"], WCC$a)
                            
                            x
                          })
  
  result <- list(reference = ref, sample = samp,
                 warped.sample = warped.sample,
                 warp.coef = WCC$a, warp.fun = NULL,
                 crit.value = WCC$v, optim.crit = "WCC",
                 warp.type = "global")
  class(result) <- c("stptw", "ptw")
  
  result
}


## ###################################################################
## reasonably brainless pieces of code

summary.stptw <- function (object, ...) {
  nsamp <- length(object$sample)
  nref <- length(object$reference)
  cat("PTW object:", object$warp.type,
      ifelse(nsamp > 1, "alignments of", "alignment of"), 
      nsamp, ifelse(nsamp > 1, "samples on", "sample on"), 
      nref, ifelse(nref > 1, "references.\n", "reference.\n"))
  cat("\nWarping coefficients:\n")
  print(coef(object))
  cat("\nWarping criterion:", object$crit.type)
  cat(ifelse(nsamp > 1, 
             "\nValues:", "\nValue:"), object$crit.value, "\n\n")
}

print.stptw <- function (x, ...) {
  nsamp <- length(x$sample)
  nref <- length(x$reference)
  cat("PTW object:", x$warp.type,
      ifelse(nsamp > 1, "alignments of", "alignment of"), 
      nsamp, ifelse(nsamp > 1, "samples on", "sample on"), 
      nref, ifelse(nref > 1, "references.\n", "reference.\n"))
}


## can we return something invisibly? Could be useful...
plot.stptw <- function(x, what = c("signal", "function"), ...) {
  what <- match.arg(what)
  
  if (what == "signal") {
    refpoints <- do.call("rbind", x$reference)
    sampoints <- do.call("rbind", x$sample)
    warpoints <- do.call("rbind", x$warped.sample)
    plot(rbind(refpoints, sampoints, warpoints)[,c("rt", "mz")],
         xlab = "Time", ylab = "m/z", pch = 1,
         col = rep(1:3, c(nrow(refpoints), nrow(sampoints),
             nrow(warpoints))), 
         ...)
    legend("topleft",
           legend = c("Reference", "Sample", "Warped sample"),
           pch = 1, col = 1:3)
  }
  if (what == "function") {
    ref.rts <- do.call("rbind", x$reference)[,"rt"]
    samp.rts <- do.call("rbind", x$sample)[,"rt"]
    warp.rts <- do.call("rbind", x$warped.sample)[,"rt"]
    all.rts <- c(ref.rts, samp.rts, warp.rts)
    
    time <- floor(min(all.rts)):ceiling(max(all.rts))
    warped.times <- warp.time(time, coef(x))
    if (!is.matrix(warped.times))
        warped.times <- matrix(warped.times, nrow = 1)
    w.time <- sweep(warped.times, 2, time)
    matplot(time, t(w.time), type = "n", xlab = "'Time'", 
            ylab = "Warped 'time' - 'time'", ...)
    abline(h = 0, col = "gray", ...)
    matlines(time, t(w.time), lty = 1, type = "l",
             col = rainbow(nrow(w.time)), 
             ...)
  }
}


## ###################################################################
## Aux functions for converting peak tables into lists of mz channels

pktab2mzchannel <- function(pktab, Ivalue = "maxo", masses = NULL,
                            nMasses = 0, massDigits = 2) {
  pkst <- pktab[, c("mz", "rt", Ivalue)]
  colnames(pkst)[3] <- "I"

  if (is.null(masses)) {
    pkst[,"mz"] <- round(pkst[,"mz"], massDigits)

    if (nMasses == 0) {
      ## we assume they are already sorted!
      masses <- unique(pkst[,"mz"])
    } else {
      massTab <- sort(table(pkst[,"mz"]), decrease = TRUE)
      masses <- as.numeric(names(massTab))[1:nMasses]
      masses <- sort(masses)
    }    
  }

  threshold <- 5*10^{-massDigits-1}
  mzdiff <- abs(outer(pkst[,"mz"], masses, "-"))
  result <- lapply(1:length(masses),
                   function(ii)
                   pkst[which(mzdiff[,ii] < threshold),,drop=FALSE])
  names(result) <- masses

  result
}

mzchannel2pktab <- function(mzchannels) {
  do.call("rbind", mzchannels)
}


