require(MetaDNA);

ions <- "./small_mgf.txt" %=>% read.mgf;
ion <- ions[[1]];

mgf.ion(ion$mz1, ion$rt, ion$ms2, 1, ion$title, ion$ms1.into)