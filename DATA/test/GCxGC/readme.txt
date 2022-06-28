R package: RGCxGC

# Read Sample chromatogram
GB08_fl <- system.file("extdata", "08GB.cdf", package = "RGCxGC")
MTBLS08 <- read_chrom(GB08_fl, mod_time = 5)
 
# Read reference chromatogram
GB09_fl <- system.file("extdata", "09GB.cdf", package = "RGCxGC")
MTBLS09 <- read_chrom(GB09_fl, mod_time = 5)

myl_d5 <- system.file("extdata", "mylbd5.CDF", package = "RGCxGC")
myl <- read_chrom(myl_d5, mod_time = 5, sam_rate = 25, verbose = F)