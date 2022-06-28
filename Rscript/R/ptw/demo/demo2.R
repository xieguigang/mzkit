### This demo explores the options for ptw warping, still for two indivual
### samples from the gaschrom dataset. 

## Again, load the gaschrom data, remove the baselines and scale:
data(gaschrom)
gaschrom <- baseline.corr(gaschrom)
gaschrom.sc <- t(apply(gaschrom, 1, function(x) {x/mean(x)}))

## Warping with default settings:
ref <- gaschrom[1,]
samp <- gaschrom[16,]
ptw.res <- ptw(ref, samp)

## Instead of using the weighted cross correlation, we could use the root
## mean square difference between chromatograms as the distance criterion 
## for warping:
ptw.res <- ptw(ref, samp, optim.crit="RMS")

## The results:
summary(ptw.res)

## The warping coefficients do not differ much from the ones found when using
## the WCC as the distance criterion. The value obtained for the warping 
## criterion differs a lot though, since we used a different criterion. If we
## want to fairly compare the results, we can use the 'try=TRUE' construction:
ptw.res <- ptw(ref, samp, init.coef=ptw.res$warp.coef, try=TRUE)
summary(ptw.res)

## Conclusion: the result is slightly less good than when using the WCC itself 
## as the distance criterion. We can also use this trick to calculate the 
## samples' dissimilarity BEFORE warping:
unwarped.res <- ptw(ref, samp, try=TRUE)
summary(unwarped.res)

## When chromatograms are strongly misaligned, it is advisable to use a larger 
## triangle width for warping. Since a triangle width of 20 is enough for the
## gaschrom data, we will show the converse, using a small triangle width:
ptw.res <- ptw(ref, samp, trwdth=1)
summary(ptw.res)

## The coefficients are more extreme now, let's see what the result looks like:
x11()
plot(ptw.res)

## Obviously, this is not what we are looking for... Note that the value for the 
## criterion cannot be directly compared with the default warping. For that, we 
## need to evaluate the result using the same triangle width. We could use the 
## 'try = TRUE' construction, but a shortcut would have been to issue:
ptw.res <- ptw(ref, samp, trwdth=1, trwdth.res=20)
summary(ptw.res)

## Finally, the triangle width is used in connection with the WCC only. When using 
## RMS as the distance criterion, Whittaker smoothing of the sample is used to 
## create a smoother optimization landscape. It can be influenced via the 
## 'smooth.param' parameter in the ptw function. For a visual flavour of what is 
## happening, the example of the 'difsm' function will serve:
example(difsm)

readline("Hit <Return> to close all graphics windows and end this demo.")
graphics.off()

