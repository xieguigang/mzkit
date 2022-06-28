predict.ptw <- function(object, newdata, 
                        what = c("response", "time"),
                        RTref = NULL, ...)
{
  what <- match.arg(what)

  switch(what,
         response = {
           if (missing(newdata)) return(object$warped.sample)

           if (!is.matrix(newdata))
             newdata <- matrix(newdata, nrow = 1)
           
           if (object$warp.type == "individual" &
               nrow(newdata) > 1 &
               nrow(newdata) != nrow(object$warp.fun))
             stop("Incorrect number of rows in newdata")

           ## if necessary, replicate the one warping function so that
           ## every row in newdata has one warping function
           if (object$warp.type == "individual") {
             WF <- object$warp.fun
           } else {
             WF <- matrix(object$warp.fun,
                          nrow(object$sample),
                          ncol(object$warp.fun), byrow = TRUE)
             
           }

           ## do the warping
           if (object$mode == "backward") {
             t(sapply(1:nrow(newdata),
                      function(i)
                        approx(x = 1:ncol(newdata), y = newdata[i,],
                               xout = WF[i,])$y))
           } else { # object$mode == "forward"
             t(sapply(1:nrow(newdata),
                      function(i)
                        approx(x = WF[i,], y = newdata[i,],
                               xout = 1:ncol(newdata))$y))
           }
         },
         time = {
           correctedTime <-
             switch(object$mode,
                    backward =
                      -sweep(object$warp.fun, 2,
                             2*(1:ncol(object$ref)), FUN = "-"),
                    object$warp.fun)
           
           if (is.null(RTref)){
             if (is.null(colnames(object$ref))) {
               RTref <- 1:ncol(object$ref)
             } else {
               RTref <- as.numeric(colnames(object$ref))
             }
           }

           if (missing(newdata)) {
             newdata <- RTref
             newdataIndices <- 1:length(RTref)
           } else {
             newdataIndices <-
                 round((newdata - min(RTref)) * (length(RTref) - 1) /
                       diff(range(RTref)) + 1)
           }

           t(sapply(1:nrow(correctedTime),
                    function(i)
                      approx(RTref, NULL, correctedTime[i, newdataIndices])$y))
         })
}
