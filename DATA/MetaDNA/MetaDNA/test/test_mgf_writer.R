xLoad("./GABA.rda");

meta.db[, "mz"] <- meta.db[, "precursor_m/z"] %=>% as.numeric;

# populate ms2 data union libpos and libneg
ms2 <- function(libname) {
	peaks <- lib.pos[[libname]];
	
	if (peaks %=>% IsNothing) {
		lib.neg[[libname]];
	} else {
		peaks;
	}
}

write.mgf(meta.db, ms2, "./GABA.mgf");