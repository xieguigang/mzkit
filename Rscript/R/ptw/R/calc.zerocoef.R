calc.zerocoef <- function (coef, zeros) {
  new_coef <- matrix(0,1,length(coef))

  for (d in (1:length(coef))) {
    for (i in (d:length(coef))) {
      new_coef[1,d] <- coef[i] * zeros^(i-d) * choose(i-1, d-1) + new_coef[1,d]
    }
  }
  
  new_coef[1,1] <- new_coef[1,1] - zeros
  
  as.numeric(new_coef)
}
