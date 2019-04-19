asciiView <- function(spectra, into.cutoff = 0.05, maxWidth = 80) {
	mz <- spectra[, 1] %=>% as.numeric;
	delta.mz <- (max(mz) - min(mz)) / (maxWidth - 10);
	into <- spectra[, 2] %=>% as.numeric;
	into <- into / max(into);
	mz <- mz[into >= into.cutoff];
	into <- into[into >= into.cutoff];		
	
	offset <- 5;
	fill.1 <- rep(" ", times = offset);
	fill.2 <- rep(" ", times = offset);
	fill.3 <- rep(" ", times = offset);
	fill.axis <- rep("-", times = offset);
	
	mzx <- min(mz);
	mzl <- 0;
	
	for(i in offset:maxWidth) {
		mzi <- (mz > mzl) & (mz <= mzx);	
		mzl <- mzx;
		mzx <- mzx + delta.mz;	
				
		if (sum(mzi) == 0) {
			# no fragment;
			# fill empty			
			fill.1 <- append(fill.1, " ");
			fill.2 <- append(fill.2, " ");
			fill.3 <- append(fill.3, " ");	
			fill.axis <- append(fill.axis, "-");			
		} else {
			int <- max(into[mzi]);
			mz.i <- mz[int == into];
			fill.axis <- append(fill.axis, "+");
			
			if (int >= 0.7) {
				# fill 123				
				fill.1 <- append(fill.1, "|");
				fill.2 <- append(fill.2, "|");
				fill.3 <- append(fill.3, "|");			
			} else if (int >= 0.3) {
				# fill 23							
				fill.1 <- append(fill.1, " ");
				fill.2 <- append(fill.2, "|");
				fill.3 <- append(fill.3, "|");			
			} else {
				# fill 3				
				fill.1 <- append(fill.1, " ");
				fill.2 <- append(fill.2, " ");
				fill.3 <- append(fill.3, "|");		
			}
		}
	}	
	
	a <- paste(fill.1, collapse = "");
	b <- paste(fill.2, collapse = "");
	c <- paste(fill.3, collapse = "");
	d <- paste(fill.axis, collapse = "");
	
	paste(c(a,b,c,d, ""), collapse = "\n");
}