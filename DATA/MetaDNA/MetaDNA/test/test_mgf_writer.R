xLoad("./GABA.rda");

meta.db         <- as.data.frame(meta.db);
meta.db[, "mz"] <- meta.db[, "precursor_m/z"] %=>% as.numeric;

# populate ms2 data union libpos and libneg
ms2 <- function(libname) {
	peaks <- lib.pos[[libname]];
		
	if (peaks %=>% IsNothing) {
		peaks <- lib.neg[[libname]];	
	}
	
	if (peaks %=>% IsNothing) {
		return(NULL);
	}
	
	peaks[, "mz"]   <- peaks[, "ProductMz"];
	peaks[, "into"] <- peaks[, "LibraryIntensity"];
	peaks;
}

write.mgf(meta.db, ms2, "./GABA.mgf");