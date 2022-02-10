select.traces <- function(X,
                          criterion = c("coda", "var", "int"),
                          window = 5, smoothing = c("median", "mean"))
{
  criterion <- match.arg(criterion)
  
  if (!is.matrix(X) & !is.array(X))
    stop("X should be a matrix or an array")
  
  if (length(dim(X)) == 3) {
    total.results <-
      apply(X, 3,
            function(x) select.traces(x, criterion, window, smoothing)$crit.val)
    crit.val <- apply(total.results, 1, prod)
  } else {
    switch(criterion,
           coda = {
             crit.val <- coda(X, window, smoothing)
           }, 
           var = {
             XX <- sweep(X, 1, apply(X, 1, function(x) sqrt(sum(x^2))),
                         FUN = "/")
             crit.val <- apply(t(XX), 2, sd)
           },
           int = {
             crit.val <- apply(X, 1, max) / max(X)
           }
           )
  }
             
  
  list(crit.val = crit.val,
       trace.nrs = order(crit.val, decreasing = TRUE))
}
