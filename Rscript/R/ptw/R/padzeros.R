padzeros <- function(data, nzeros, side="both") {

if (is.vector(data)==TRUE) data <- t(as.matrix(data))

M.zeros <- matrix(0, nrow(data), nzeros)

newdata <- switch(side, 
	   both  = cbind(M.zeros, data, M.zeros),
	   left = cbind(M.zeros, data),
	   right   = cbind(data, M.zeros))

}
