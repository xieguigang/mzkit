# mzDeco

Extract peak and signal data from rawdata
 
 Data processing is the computational process of converting raw LC-MS 
 data to biological knowledge and involves multiple processes including 
 raw data deconvolution and the chemical identification of metabolites.
 
 The process of data deconvolution, sometimes called peak picking, is 
 in itself a complex process caused by the complexity of the data and 
 variation introduced during the process of data acquisition related to 
 mass-to-charge ratio, retention time and chromatographic peak area.

+ [read.xcms_peaks](mzDeco/read.xcms_peaks.1) read the peaktable file that in xcms2 output format
+ [peak_subset](mzDeco/peak_subset.1) make sample column projection
+ [find_xcms_ionPeaks](mzDeco/find_xcms_ionPeaks.1) 
+ [mz_deco](mzDeco/mz_deco.1) Chromatogram data deconvolution
+ [write.peaks](mzDeco/write.peaks.1) write peak debug data
+ [read.peakFeatures](mzDeco/read.peakFeatures.1) read the peak feature table data
+ [peak_alignment](mzDeco/peak_alignment.1) Do COW peak alignment and export peaktable
+ [mz.groups](mzDeco/mz.groups.1) do ``m/z`` grouping under the given tolerance
+ [xic_pool](mzDeco/xic_pool.1) 
+ [pull_xic](mzDeco/pull_xic.1) extract a collection of xic data for a specific ion feature
