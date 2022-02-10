asysm <- function (y, lambda = 1e+07, p = 0.001, eps = 1e-8, maxit = 25) {
  z <- 0 * y
  w <- z + 1

  eps <- max(eps, eps*diff(range(y)))
  for (it in 1:maxit) {
    zold <- z
    z <- whit2(y, lambda, w)
    w <- p * (y > z) + (1 - p) * (y <= z)
    dz <- max(abs(z - zold))
    if (dz < eps) break
  }

  if (dz >= eps)
      warning("Function asysm did not reach convergence")

  return(z)
}
