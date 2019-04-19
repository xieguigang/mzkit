char_display <- function(spectra, into.cutoff = 0.05, maxWidth = 120) {
	mz <- spectra[, 1] %=>% as.numeric;
	into <- spectra[, 2] %=>% as.numeric;
	into <- into / max(into);
	mz <- mz[into >= into.cutoff];
	into <- into[into >= into.cutoff];
	
	fill.1 <- c();
	fill.2 <- c();
	fill.3 <- c();
	fill.axis <- c();
	
	
}