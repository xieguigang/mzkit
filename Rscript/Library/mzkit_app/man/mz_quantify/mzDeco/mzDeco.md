﻿# mzDeco

Extract peak and signal data from rawdata
 
 Data processing is the computational process of converting raw LC-MS 
 data to biological knowledge and involves multiple processes including 
 raw data deconvolution and the chemical identification of metabolites.
 
 The process of data deconvolution, sometimes called peak picking, is 
 in itself a complex process caused by the complexity of the data and 
 variation introduced during the process of data acquisition related to 
 mass-to-charge ratio, retention time and chromatographic peak area.

+ [read.xcms_peaks](mzDeco/read.xcms_peaks.1) read the peaktable file that in xcms2 output format
+ [to_expression](mzDeco/to_expression.1) cast peaktable to expression matrix object
+ [to_matrix](mzDeco/to_matrix.1) cast peaktable to mzkit expression matrix
+ [write.xcms_peaks](mzDeco/write.xcms_peaks.1) save mzkit peaktable object to csv table file
+ [as.peak_set](mzDeco/as.peak_set.1) cast dataset to mzkit peaktable object
+ [read.xcms_features](mzDeco/read.xcms_features.1) Try to cast the dataframe to the mzkit peak feature object set
+ [peak_subset](mzDeco/peak_subset.1) make sample column projection
+ [xcms_peak](mzDeco/xcms_peak.1) Create a xcms peak data object
+ [find_xcms_ionPeaks](mzDeco/find_xcms_ionPeaks.1) helper function for find ms1 peaks based on the given mz/rt tuple data
+ [filter_noise_spectrum](mzDeco/filter_noise_spectrum.1) make filter of the noise spectrum data
+ [get_xcms_ionPeaks](mzDeco/get_xcms_ionPeaks.1) get ion peaks via the unique reference id
+ [adjust_to_seconds](mzDeco/adjust_to_seconds.1) adjust the reteintion time data to unit seconds
+ [RI_reference](mzDeco/RI_reference.1) Create RI reference dataset.
+ [RI_cal](mzDeco/RI_cal.1) RI calculation of a speicifc sample data
+ [mz_deco](mzDeco/mz_deco.1) Chromatogram data deconvolution
+ [MS1deconv](mzDeco/MS1deconv.1) A debug function for test peak finding method
+ [read.rt_shifts](mzDeco/read.rt_shifts.1) 
+ [write.peaks](mzDeco/write.peaks.1) write peak debug data
+ [read.peakFeatures](mzDeco/read.peakFeatures.1) read the peak feature table data
+ [peak_alignment](mzDeco/peak_alignment.1) Do COW peak alignment and export peaktable
+ [RI_batch_join](mzDeco/RI_batch_join.1) make peaktable join of two batch data via (mz,RI)
+ [mz.groups](mzDeco/mz.groups.1) do ``m/z`` grouping under the given tolerance
+ [rt_groups](mzDeco/rt_groups.1) Make peaks data group merge by rt directly
+ [xic_pool](mzDeco/xic_pool.1) Load xic sample data files
+ [pull_xic](mzDeco/pull_xic.1) extract a collection of xic data for a specific ion feature
