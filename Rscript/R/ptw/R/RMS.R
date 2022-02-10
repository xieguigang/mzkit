RMS <- function(warp.coef, ref, samp, B, mode)
{
  w <- B %*% warp.coef

  interp <- warp.sample(samp, w, mode)
  if (nrow(ref) == 1) {
    ref <- c(ref)
  } else {
    ref <- t(ref)
  }
  r <- interp - ref
  
  sqrt(mean(r^2, na.rm = TRUE))
}
