### Next to aligning a single sample with a single reference, the ptw package
### offers the possibility to align multiple samples at once or to apply
### global warping - warping multiple chromatic traces with a single warping 
### function - which is of use in aligning LC-MS data for instance.

## Again, let's start with the gaschrom data, remove the baselines and scale:
data(gaschrom)
gaschrom <- baseline.corr(gaschrom)
gaschrom.sc <- t(apply(gaschrom, 1, function(x) {x/mean(x)}))

## If we want to align chromatograms 2:16 with chromatogram 1, we issue:
ptw.res <- ptw(gaschrom[1,], gaschrom[2:16,])

summary(ptw.res)

x11()
plot(ptw.res)

x11()
plot(ptw.res, what="function")

## If we want to align pairs of chromatograms, e.g. 2 with 1, 4 with 3, etc., 
## we issue:
ptw.res <- ptw(gaschrom[2*(1:8)-1,], gaschrom[2*(1:8),])

summary(ptw.res)

x11()
plot(ptw.res)

x11()
plot(ptw.res, what="function")

## Obviously, these are just convenient ways of doing something like:

## Not run:
#ptw.res <- list()
#for (k in 1:8) {
#  ptw.res[[k]] <- ptw(gaschrom[2*k-1, ], gaschrom[2*k, ])
#}

## But now for something different. In LC-MS data, samples are matrices 
## rather than vectors. Aligning these with ptw can be done using a combination 
## of global warping and individual warping (see Bloemberg et al. 2010):

## Load the 'lcms' data
data(lcms)

## These data do not suffer from an apparent baseline; just scaling will suffice:
lcms.sc <- aperm(apply(lcms, c(1,3),
                       function(x) x/mean(x) ), c(2,1,3))

## Pad the chromatograms with zeros
lcms.sc.zp <- aperm(apply(lcms.sc, c(1,3),
                        function(x) padzeros(x, 1000) ), c(2,1,3))

## Using global warping, we warp the full LC-MS matrix with a single 
## cubic warping function (this takes a little while):
warp1 <- ptw(lcms.sc.zp[,,2], lcms.sc.zp[,,3], trwdth = 100, 
	      warp.type="global", init.coef = c(0,1,0,0))
summary(warp1)

## We can easily calculate the warping coefficients for the original 
## data (without zero-padding):
Coefs <- calc.zerocoef(warp1$warp.coef, 1000)

## And these can be applied to the un-zero-padded data using the 'try = TRUE' trick:
warp1.origdata <- ptw(lcms.sc[,,2], lcms.sc[,,3], warp.type="global",
		      init.coef = Coefs, try=TRUE)
summary(warp1.origdata)

## For visualization, we will plot the sum chromatograms, known as 'Total Ion
## Currents' or TICs for short:
layout(matrix(1:2,2,1, byrow=TRUE))
plot(colSums(warp1.origdata$reference), type="l", main="TIC: original data", ylim=c(0,550))
lines(colSums(warp1.origdata$sample), type="l", col="red", lty=5)
plot(colSums(warp1.origdata$reference), type="l", main="TIC: warped data", ylim=c(0,550))
lines(colSums(warp1.origdata$warped.sample), type="l", col="steelblue3", lty=5)

## As a numerical comparison, we should compare the values of the distance criterion for
## the unwarped and warped matrices, rather than for the TICs:
warp0.origdata <- ptw(lcms.sc[,,2], lcms.sc[,,3], warp.type="global", try=TRUE)
summary(warp0.origdata)

readline("Hit <Return> to close all graphics windows and end this demo.")
graphics.off()

