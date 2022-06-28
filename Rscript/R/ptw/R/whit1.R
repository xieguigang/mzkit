whit1 <- function(y, lambda, w = rep(1, ny))
{
  ny <- length(y)
  z <- d <- e <- rep(0, length(y))
  
  .C("smooth1",
     w = as.double(w),
     y = as.double(y),
     z = as.double(z),
     lamb = as.double(lambda),
     mm = as.integer(length(y)),
     d = as.double(d),
     e = as.double(e),
     PACKAGE = "ptw")$z
}

