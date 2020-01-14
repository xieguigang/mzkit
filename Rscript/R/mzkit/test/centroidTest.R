ions <- read.mgf("D:/MassSpectrum-toolkits/DATA/test/HCD_profiles.txt");

profile <- ions[[1]]$ms2;

ms2 <- centroid.2(profile, angle.threshold = 0.1);
write.csv(ms2, file ="D:/MassSpectrum-toolkits/Rscript/demo/mz_centroid.csv", row.names = FALSE);