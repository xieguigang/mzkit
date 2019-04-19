
#' ASCII View of Spectra Matrix
#'
asciiView <- function(spectra, 
	into.cutoff = 0.05, 
	maxWidth = 80, 
	show.mz.labels = TRUE) {
	
	# Extract spectra matrix data
	mz <- spectra[, 1] %=>% as.numeric;
	# column steps in viewer
	delta.mz <- (max(mz) - min(mz)) / (maxWidth - 10);
	# cleanup matrix
	into <- spectra[, 2] %=>% as.numeric;
	into <- into / max(into);
	mz <- mz[into >= into.cutoff];
	into <- into[into >= into.cutoff];		
	
	offset <- 5;
	fill.1 <- rep(" ", times = offset);
	fill.2 <- rep(" ", times = offset);
	fill.3 <- rep(" ", times = offset);
	fill.4 <- rep(" ", times = offset);
	fill.axis <- rep("-", times = offset);
	labels <- list();
	
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
			fill.4 <- append(fill.4, " ");	
			fill.axis <- append(fill.axis, "-");			
		} else {
			int <- max(into[mzi]);
			mz.i <- mz[int == into];
			fill.axis <- append(fill.axis, "+");
			labels[[as.character(i)]] <- mz.i;
			
			if (int >= 0.85) {
				# fill 123				
				fill.1 <- append(fill.1, "|");
				fill.2 <- append(fill.2, "|");
				fill.3 <- append(fill.3, "|");		
				fill.4 <- append(fill.4, "|");						
			} else if (int >= 0.6) {
				# fill 23							
				fill.1 <- append(fill.1, " ");
				fill.2 <- append(fill.2, "|");
				fill.3 <- append(fill.3, "|");		
				fill.4 <- append(fill.4, "|");								
			} else if (int >= 0.4) {
				# fill 23							
				fill.1 <- append(fill.1, " ");
				fill.2 <- append(fill.2, " ");
				fill.3 <- append(fill.3, "|");		
				fill.4 <- append(fill.4, "|");								
			} else {
				# fill 3				
				fill.1 <- append(fill.1, " ");
				fill.2 <- append(fill.2, " ");
				fill.3 <- append(fill.3, " ");		
				fill.4 <- append(fill.4, "|");				
			}
		}
	}	
	
	a <- paste(fill.1, collapse = "");
	b <- paste(fill.2, collapse = "");
	c <- paste(fill.3, collapse = "");
	d <- paste(fill.4, collapse = "");
	e <- paste(fill.axis, collapse = "");
	
	views <- c("", a,b,c,d,e);
	
	
	
	paste(append(view, ""), collapse = "\n");
}