#' Write ms2 data as a mgf spectrum data file.
file.mgf <- function(AnnoDataSet, isotope) {

	path <- sprintf("%s/sirius/%s.mgf", AnnoDataSet$outputdir, paste(isotope, collapse = "-"));
	peaktable <- AnnoDataSet$peaktable;
	
	# 生成头部的MS1信息	
	ms1 <- c();
	ms1[1] <- "BEGIN IONS";
	ms1[2] <- sprintf("PEPMASS=%s", getSingleValue(peaktable[isotope[1]], "mz"));
	ms1[3] <- "MSLEVEL=1";
	ms1[4] <- "CHARGE=1+";
	
	i <- 5;
	
	for (n in isotope) {
		n <- peaktable[n, ];
		ms1[i] <- sprintf("%s %s", getSingleValue(n, "mz"), getSingleValue(n, "into"));
		i = i + 1;
	}
	
	ms1[i] <- "END IONS";
	ms1[i+1] <- "";
	
	# 在后面写入每一个峰的MS2数据
	ms2 <- c();
	
	for (n in isotope) {
		data <- c();	
		
		data[1] <- "BEGIN IONS";
		data[2] <- sprintf("PEPMASS=%s", getSingleValue(peaktable[n, ], "mz"));
		data[3] <- "MSLEVEL=2";
		data[4] <- "CHARGE=1+";
		
		data <- append(
			data, 
			unlist(
			lapply(get.ms2peaks(AnnoDataSet$peak_ms2, n), 
			function(mz) {
			return(sprintf("%s %s", getSingleValue(mz, "mz"), getSingleValue(mz, "intensity")));
		})));
		
		data <- append(data, c("END IONS"));
		
		ms2 <- append(ms2, "");
		ms2 <- append(ms2, data);
	}
	
	# 将ms1和并ms2的数据，构成最终需要写入文件的mgf文件数据
	mgf <- append(ms1, ms2);
	
	file <- file(path);
	writeLines(mgf, file);
	close(file);
		
	return(path);
}

#' Read a given mgf file
#'
#' @return Returns a list of mgf ions that parsed from the given mgf spectrum data file.
read.mgf <- function(fileName) {
	lines  <- fileName %=>% ReadAllLines;
	ions   <- list();
	buffer <- c();
	index  <- 1;
	
	for(i in 1:length(lines)) {
		line   <- lines[i];
		buffer <- append(buffer, line);
		
		if (line == "END IONS") {
			ions[[index]] <- buffer %=>% parse.mgf;
			buffer        <- c();
			index         <- index + 1;
		}
	}
	
	if (length(buffer) > 0) {
		ions[[index]] <- buffer %=>% parse.mgf;
	}
	
	ions;
}

#' Parse mgf ion data from text buffer.
#' 
parse.mgf <- function(buffer) {	

	# The first line of the buffer is BEGIN IONS
	# so start from the second line
	i    <- 2;
	meta <- list();
	
	while(TRUE) {
		p <- InStr(buffer[i], "=");
		
		if (p > 0) {
			name  <- substr(buffer[i], 1, p - 1);
			value <- substring(buffer[i], p + 1);
			meta[[name]] <- value;
			
			i <- i + 1;
		} else {
			break;
		}
	}
	
	mz   <- c();
	into <- c();
	
	# The last line of the buffer is END IONS
	# so end before reach the last line
	for (j in i:(length(buffer) - 1)) {
		tokens <- strsplit(buffer[j] %=>% Trim, "\\s+")[[1]];
		
		if (length(tokens) != 2) {
		    stop("Incorrect file format!");
		} else {
			mz   <- append(mz, tokens[1]);
			into <- append(into, tokens[2]);
		}
	}
	
	ms2 <- data.frame(mz = mz, into = into);
	mz  <- strsplit(meta[["PEPMASS"]], "\\s+")[[1]];
	
	list(mz1      = mz[1]                 %=>% as.numeric, 
		 ms1.into = mz[2]                 %=>% as.numeric, 
		 rt       = meta[["RTINSECONDS"]] %=>% as.numeric, 
		 title    = meta[["TITLE"]], 
		 charge   = meta[["CHARGE"]], 
		 ms2      = ms2
	);
}