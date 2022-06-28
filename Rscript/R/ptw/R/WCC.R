WCC <- function(warp.coef, ref, samp, B, trwdth = 20, wghts, mode,
                ref.acors = NULL)
{
  if (missing(wghts))
    wghts <- 1 - (0:trwdth)/trwdth

  if (is.null(ref.acors))
    ref.acors <- apply(ref, 1, wac, trwdth = trwdth, wghts = wghts)

  w <- B %*% warp.coef
  interp <- warp.sample(samp, w, mode)
  
  wccs <- sapply(1:ncol(interp),
                 function(i) {
                   wcc(ref[i, !is.na(interp[,i])],
                       interp[!is.na(interp[,i]), i],
                       trwdth = trwdth,
                       wghts = wghts,
                       acors1 = ref.acors[i])
                 })
  
  1 - mean(wccs) # so that an optimal value is zero
}

wac <- function(pattern1, trwdth, wghts = NULL)
{
  if (is.null(wghts)) 
    wghts <- 1 - (0:trwdth)/trwdth

  .C("wacdist",
     as.double(pattern1),
     as.integer(length(pattern1)),
     as.double(wghts),
     as.integer(trwdth),
     wacval = double(1),
     PACKAGE = "ptw")$wacval
}


wcc <- function(pattern1, pattern2, trwdth,
                wghts = NULL, acors1 = NULL, acors2 = NULL)
{
  if (is.null(wghts))
    wghts <- 1 - (0:trwdth)/trwdth
  
  if (is.null(acors1))
    acors1 <- wac(pattern1, trwdth, wghts)
  if (is.null(acors2))
    acors2 <- wac(pattern2, trwdth, wghts)

  .C("wccdist",
     as.double(pattern1),
     as.double(pattern2),
     as.integer(length(pattern1)),
     as.double(wghts),
     as.integer(trwdth),
     crossterm = double(1),
     PACKAGE = "ptw")$crossterm / (acors1*acors2)
}
