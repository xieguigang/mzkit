difsm <- function(y, lambda)
{
  ny <- length(y)
  w <- rep(1, ny)
  z <- d <- c <- e <- rep(0, length(y))
  
  .C("smooth2",
     w = as.double(w),
     y = as.double(y),
     z = as.double(z),
     lamb = as.double(lambda),
     mm = as.integer(length(y)),
     d = as.double(d),
     c = as.double(c),
     e = as.double(e),
     PACKAGE = "ptw")$z
}

