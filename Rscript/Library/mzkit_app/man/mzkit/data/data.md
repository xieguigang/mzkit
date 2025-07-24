# data

Provides core functionality for mass spectrometry data processing and analysis within the mzkit framework.
> This module contains operations for:
>  
>  1. Mass spectral data manipulation (MS1 and MS2 level)
>  2. Chromatographic data processing (XIC/TIC generation)
>  3. Spectral similarity calculations and alignment
>  4. Data format conversions between native objects and R dataframe
>  5. Spectral library operations and metadata handling
>  
>  Key features include:
>  
>  - SPLASH ID generation for spectral uniqueness verification
>  - ROI-based spectral grouping and analysis
>  - Raw data centroiding and intensity normalization
>  - Cross-sample spectral alignment and matching
>  - Mass tolerance-aware operations (ppm/DA)

+ [splash_id](data/splash_id.1) Calculates the SPLASH identifier for the given spectrum data.
+ [unionPeaks](data/unionPeaks.1) Union and merge the given multiple spectrum data into one single spectrum
+ [representative](data/representative.1) Generates a representative spectrum by aggregating (sum or mean) input spectra.
+ [nsize](data/nsize.1) Gets the number of fragments in a spectrum object.
+ [search](data/search.1) search the target query spectra against a reference mzpack data file
+ [alignment_ref](data/alignment_ref.1) get alignment result tuple: query and reference
+ [alignment_str](data/alignment_str.1) Creates a formatted string representation of aligned peaks.
+ [peakMs2](data/peakMs2.1) Constructs a PeakMs2 object from spectral data.
+ [groupBy_ROI](data/groupBy_ROI.1) make a tuple list via grouping of the spectrum data via the ROI id inside the metadata list
+ [libraryMatrix](data/libraryMatrix.1) Create a library matrix object
+ [XIC_groups](data/XIC_groups.1) Groups MS1 scans into XIC (Extracted Ion Chromatogram) groups by m/z.
+ [XIC](data/XIC.1) Extracts chromatogram data for a specific m/z from MS1 scans.
+ [rt_slice](data/rt_slice.1) Filters MS1 scans within a specified retention time window.
+ [intensity](data/intensity.1) Extracts intensity values from MS1 scans or PeakMs2 spectra.
+ [scan_time](data/scan_time.1) Extracts retention times from MS1 scans or PeakMs2 spectra.
+ [make.ROI_names](data/make.ROI_names.1) Generates unique ROI (Region of Interest) IDs for spectra.
+ [read.MsMatrix](data/read.MsMatrix.1) Reads a spectral matrix from a CSV file.
+ [linearMatrix](data/linearMatrix.1) Generates a string representation of top intensity ions from spectra.
+ [logfc](data/logfc.1) use log foldchange for compares two spectrum
+ [msn_matrix](data/msn_matrix.1) 
