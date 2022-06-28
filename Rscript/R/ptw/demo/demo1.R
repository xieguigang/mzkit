### Let's apply ptw to two samples from the 'gaschrom' dataset that is 
### included in the package.

## Load the gaschrom data:
data(gaschrom)

## It is important to remove baselines before warping.
## A baseline correction algorithm based on asymmetric least squares
## is included in the ptw package:
gaschrom <- baseline.corr(gaschrom)

## Although the gaschrom samples do not differ wildly in intensity,
## it is a good idea, generally, to scale the data
gaschrom.sc <- t(apply(gaschrom, 1, function(x) {x/mean(x)}))

## First, we apply ptw to the first and last gas chromatograms, using the 
## default settings (quadratic warping function, weighted cross correlation 
## (WCC) as the distance criterion and a corresponding triangle width 
## of 20 points:
ref <- gaschrom[1,]
samp <- gaschrom[16,]
ptw.res <- ptw(ref, samp)

## Let's look at the results:
summary(ptw.res)

## The warping coefficients signify a constant shift, linear and quadratic 
## stretch / compression, respectively. The similarity between reference 
## and warped sample is better if the warping criterion is closer to zero.

## Let's inspect the results visually:
x11()
plot(ptw.res)

## And the warping function:
x11()
plot(ptw.res, what="function")

## The enhancement is obvious, but we can try to improve a bit more by using 
## a higher order warping function. This is done by specifying more 
## coefficients. Let's try a cubic warping function:

ptw.res <- ptw(ref, samp, init.coef=c(0,1,0,0)) # Default is c(0,1,0)

## Again, let's look at the results:
summary(ptw.res)

## As expected, the warping criterion (WCC) is closer to zero now. 
## Compare the warped chromatograms (the most notable difference is the 
## rightmost peak):

x11()
plot(ptw.res)

x11()
plot(ptw.res, what="function")

## More information about the customization of ptw warping can be found in demo2

readline("Hit <Return> to close all graphics windows and end this demo.")
graphics.off()
