baseline.corr <- function(y, ...)
{
  if (is.vector(y)) {
    y - asysm(y, ...)
  } else {
    t(apply(y, 1, function(x) x - asysm(x, ...)))
  }
}
