coda <- function(x, window = 5, smoothing = c("median", "mean"))
{
  smoothing <- match.arg(smoothing)
  
  if (is.vector(x)) x <- matrix(x, nrow = 1)

  x.smooth <-
    switch(smoothing,
           mean = t(apply(x, 1, function(xx) rowMeans(embed(xx, window)))),
           median = t(apply(x, 1, runmed, k = window, endrule = "keep")))
  
  ## cut the first and last couple of variables; with mean smoothing
  ## these are already left out of the smoothed matrix
  nc <- ncol(x)
  noff <- window %/% 2
  if (smoothing == "median")
    x.smooth <- x.smooth[, -c(1:noff, (nc - noff + 1): nc), drop = FALSE]

  x <- x[, -c(1:noff, (nc - noff + 1): nc), drop = FALSE]
  lambda <- sqrt(rowSums(x^2,na.rm = TRUE)) 
  A.lambda  <- sweep(x, MARGIN=1, STATS=lambda, FUN="/")
  A.s  <- t(scale(t(x.smooth), center=TRUE, scale=TRUE)) 

  rowSums(A.lambda * A.s) / sqrt(nc - window)
}

