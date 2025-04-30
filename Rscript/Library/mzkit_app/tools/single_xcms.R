library(xcms);

raw_files = list.files(
  pattern = "\\.mzXML$|\\.mzML$", 
  full.names = TRUE               
);
xcms_work = "./xcms_peaks";

dir.create(xcms_work);

print("processing raw data files:");
print(raw_files);
cat("\n\n");

for (file in raw_files) {
    print(sprintf("findPeaks: %s", file));

    data   <- xcmsRaw(file);
    xpeaks <- findPeaks(data,method="matchedFilter",fwhm=15);
    xpeaks <- as.data.frame(xpeaks);
    file <- sub("\\..*$", "", basename(file));
    file <- file.path(xcms_work, sprintf("%s.csv", file));

    print(sprintf("    => %s", file));
    # dump peaks data
    write.csv(xpeaks, file, row.names = FALSE);
}