# 函数生成sirius预处理计算所需要的mgf文件
# 函数的返回值为生成的mgf文件的文件路径
# @param isotope 经过peaktable计算所认为的可能的同位素峰的列表，即peaktable之中的行编号向量
#                要求的格式为：第一个元素为正常的分子，剩下的都是递增的同位素分子
#                isotope所获取得到的行数据之中的rt应该都是一样的，但是每一个元素的mz都相差1
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

read.mgf <- function(fileName) {
	lines <- fileName %=>% ReadAllLines;
	
	
}