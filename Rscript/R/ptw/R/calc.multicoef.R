calc.multicoef <- function(coef1, coef2) {
  coef1 <- as.numeric(coef1)
  coef2 <- as.numeric(coef2)
  
  new_coef <- matrix(0, nrow = 1, ncol = (((length(coef1)-1) * (length(coef2)-1))+1))
  
  powerlist <- list()
  powerlist[[1]] <- matrix(0, nrow = 1, ncol = (((length(coef1)-1) *
                                                     (length(coef2)-1))+1))
  powerlist[[1]][1,1] <- 1
  
  for (i in 1:(length(coef1)-1)) {
    multilist <- lapply(1:length(coef2), function(j) 0:i)
    ## multilist <- list()
    
    ## for (j in 1:length(coef2)) {
    ##   multilist[[j]] <- 0:i
    ## }
    
    expmat <- as.matrix(do.call(expand.grid,
                                multilist)[which(rowSums(do.call(expand.grid, multilist)) == i),])
    rownames(expmat) <- NULL
    colnames(expmat) <- NULL

    ## Vector indicating the 'power' of every row of matrix expmat
    powervec <- colSums(t(expmat) * c(0 : (dim(expmat)[2]-1))) 
    
    coefmat <- matrix(0, 1, (((length(coef1)-1) * (length(coef2)-1))+1))
    
    mypower <- function(vec1, vec2) {
      ##Power function that doesn't count x^0
      result <- vec1^vec2
      result[which(vec2 == 0)] <- NA
      result
    }
    
    choosen <- function(n, k) {
      ## Function to calculate numbers from Pascal's simplex
      ## (multinomial coefficients) 
      k <- as.matrix(k)
      k[is.na(k)] <- 0
      
      if (n==0) {
        rep(1, dim(k)[2])
      } else {
        factorial(n) / prod(apply(k, 1, factorial))
      }
    }
    
    for (k in 1:length(powervec)) {
      coefmat[powervec[k]+1] <- coefmat[powervec[k]+1] +
          choosen(i, expmat[k,]) *prod(mypower(coef2, expmat[k,]), na.rm=TRUE)
    }
    
    powerlist[[i+1]] <- coefmat
    
  }
  
  for (i in 1:((length(coef1)-1) * (length(coef2)-1)+1)) {
    for (j in 1:length(powerlist)) {
      new_coef[1,i] <- new_coef[i] + coef1[j] * powerlist[[j]][1,i]
    }
  }

  as.numeric(new_coef)
}
