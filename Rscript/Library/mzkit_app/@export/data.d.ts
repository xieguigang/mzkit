// export R# package module type define for javascript/typescript language
//
//    imports "data" from "mzkit";
//
// ref=mzkit.data@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Provides core functionality for mass spectrometry data processing and analysis within the mzkit framework.
 * 
 * > This module contains operations for:
 * >  
 * >  1. Mass spectral data manipulation (MS1 and MS2 level)
 * >  2. Chromatographic data processing (XIC/TIC generation)
 * >  3. Spectral similarity calculations and alignment
 * >  4. Data format conversions between native objects and R dataframe
 * >  5. Spectral library operations and metadata handling
 * >  
 * >  Key features include:
 * >  
 * >  - SPLASH ID generation for spectral uniqueness verification
 * >  - ROI-based spectral grouping and analysis
 * >  - Raw data centroiding and intensity normalization
 * >  - Cross-sample spectral alignment and matching
 * >  - Mass tolerance-aware operations (ppm/DA)
*/
declare namespace data {
   /**
    * get alignment result tuple: query and reference
    * 
    * 
     * @param align AlignmentOutput object from spectral matching.
     * @param query Name label for query spectrum.
     * 
     * + default value Is ``'Query'``.
     * @param reference Name label for reference spectrum.
     * 
     * + default value Is ``'Reference'``.
     * @return a tuple list object that contains spectrum alignment result:
     *  
     *  1. query - spectrum of sample query
     *  2. reference - spectrum of library reference
   */
   function alignment_ref(align: object, query?: string, reference?: string): object;
   /**
    * Creates a formatted string representation of aligned peaks.
    * 
    * 
     * @param mz Array of m/z values for aligned peaks.
     * @param query Array of query intensities.
     * @param reference Array of reference intensities.
     * @param annotation Optional annotations for peaks.
     * 
     * + default value Is ``null``.
     * @return Formatted string showing alignment details.
   */
   function alignment_str(mz: any, query: any, reference: any, annotation?: any): string;
   /**
    * make a tuple list via grouping of the spectrum data via the ROI id inside the metadata list
    * 
    * 
     * @param peakms2 a collection of the spectrum data to make spectrum data grouping.
     * @param default the default ROI id for make the data groups if the metadata 
     *  is null or the ``ROI`` id tag is missing from the spectrum object metadata.
     * 
     * + default value Is ``'Not_Assigned'``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A tuple list object that contains the spectrum data grouping by the ROI id.
     *  
     *  The key of the tuple list is the ROI id, and the value is a list of spectrum data
     *  that belongs to this ROI id.
     *  
     *  If no ROI id was assigned, a warning message will be added to the runtime environment.
   */
   function groupBy_ROI(peakms2: any, default?: string, env?: object): object;
   /**
    * Extracts intensity values from MS1 scans or PeakMs2 spectra.
    * 
    * 
     * @param ticks Input scans or spectra.
     * @param mz Optional m/z to extract specific intensity.
     * 
     * + default value Is ``-1``.
     * @param mzdiff Mass tolerance for m/z matching.
     * 
     * + default value Is ``'da:0.3'``.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return Numeric vector of intensity values.
   */
   function intensity(ticks: any, mz?: number, mzdiff?: any, env?: object): number;
   /**
    * Create a library matrix object
    * 
    * 
     * @param matrix for a dataframe object, should contains column data:
     *  mz, into and annotation.
     * 
     * + default value Is ``null``.
     * @param title -
     * 
     * + default value Is ``'MS Matrix'``.
     * @param parentMz 
     * + default value Is ``-1``.
     * @param centroid 
     * + default value Is ``false``.
     * @param args 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A simple mzkit spectrum peak list object
   */
   function libraryMatrix(matrix?: any, title?: string, parentMz?: number, centroid?: boolean, args?: object, env?: object): any;
   /**
    * Generates a string representation of top intensity ions from spectra.
    * 
    * 
     * @param data Input PeakMs2 spectra.
     * @param topIons Number of top ions to include per spectrum.
     * 
     * + default value Is ``5``.
     * @return String array formatted as "mz:intensity" for top ions.
   */
   function linearMatrix(data: object, topIons?: object): string;
   /**
    * use log foldchange for compares two spectrum
    * 
    * 
     * @param spec1 -
     * @param spec2 -
     * @param da 
     * + default value Is ``0.03``.
   */
   function logfc(spec1: object, spec2: object, da?: number): any;
   module make {
      /**
       * Generates unique ROI (Region of Interest) IDs for spectra.
       * 
       * 
        * @param ROIlist Input PeakMs2 spectra or list with mz/rt vectors.
        * @param name_chrs If true, returns ROI IDs as strings; otherwise updates PeakMs2 metadata.
        * 
        * + default value Is ``false``.
        * @param prefix Prefix for ROI IDs.
        * 
        * + default value Is ``''``.
        * @param env R environment for error handling.
        * 
        * + default value Is ``null``.
        * @return String array of ROI IDs or modified PeakMs2 objects array.
      */
      function ROI_names(ROIlist: any, name_chrs?: boolean, prefix?: string, env?: object): object|string;
   }
   /**
    * Gets the number of fragments in a spectrum object.
    * 
    * 
     * @param matrix A LibraryMatrix or PeakMs2 object.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return Integer count of fragments.
   */
   function nsize(matrix: any, env?: object): object;
   /**
    * Constructs a PeakMs2 object from spectral data.
    * 
    * 
     * @param precursor Precursor m/z value.
     * @param rt Retention time in seconds.
     * @param mz Array of fragment m/z values.
     * @param into Array of fragment intensities.
     * @param totalIons Total ion current (optional).
     * 
     * + default value Is ``0``.
     * @param file Source file identifier.
     * 
     * + default value Is ``null``.
     * @param libname Library identifier.
     * 
     * + default value Is ``null``.
     * @param precursor_type Precursor adduct type.
     * 
     * + default value Is ``null``.
     * @param meta Metadata list for the peak.
     * 
     * + default value Is ``null``.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A PeakMs2 object containing the spectral data.
   */
   function peakMs2(precursor: number, rt: number, mz: number, into: number, totalIons?: number, file?: string, libname?: string, precursor_type?: string, meta?: object, env?: object): object;
   module read {
      /**
       * Reads a spectral matrix from a CSV file.
       * 
       * 
        * @param file Path to CSV file containing spectral data.
        * @return Array of Library objects parsed from the file.
      */
      function MsMatrix(file: string): object;
   }
   /**
    * Generates a representative spectrum by aggregating (sum or mean) input spectra.
    * 
    * 
     * @param x Input spectra (PeakMs2 or LibraryMatrix collection).
     * @param mean If true, uses mean intensity; otherwise sums intensities.
     * 
     * + default value Is ``true``.
     * @param centroid Mass tolerance for centroiding peaks.
     * 
     * + default value Is ``0.1``.
     * @param intocutoff Relative intensity cutoff (0-1) to filter weak peaks.
     * 
     * + default value Is ``0.05``.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A LibraryMatrix representing the aggregated spectrum.
   */
   function representative(x: any, mean?: boolean, centroid?: number, intocutoff?: number, env?: object): object;
   /**
    * Filters MS1 scans within a specified retention time window.
    * 
    * 
     * @param ms1 Input MS1 scan data.
     * @param rtmin Minimum retention time.
     * @param rtmax Maximum retention time.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return Array of MS1 scans within the RT window.
   */
   function rt_slice(ms1: any, rtmin: number, rtmax: number, env?: object): object;
   /**
    * Extracts retention times from MS1 scans or PeakMs2 spectra.
    * 
    * 
     * @param ticks Input scans or spectra.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return Numeric vector of retention times.
   */
   function scan_time(ticks: any, env?: object): number;
   /**
    * search the target query spectra against a reference mzpack data file
    * 
    * 
     * @param q The target spectra data, mz and into data fields must 
     *  be included inside if this parameter value is a dataframe.
     * @param refer A mzpack data object that contains the reference 
     *  spectrum dataset. The spectra dataset inside this mzpack data object
     *  must be already been centroid processed!
     * @param tolerance 
     * + default value Is ``'da:0.3'``.
     * @param intocutoff 
     * + default value Is ``0.05``.
     * @param similarity_cutoff 
     * + default value Is ``0.3``.
     * @param env 
     * + default value Is ``null``.
   */
   function search(q: any, refer: object, tolerance?: any, intocutoff?: number, similarity_cutoff?: number, env?: object): object;
   /**
    * Calculates the SPLASH identifier for the given spectrum data.
    * 
    * > The SPLASH is an unambiguous, database-independent spectral identifier, 
    * >  just as the InChIKey is designed to serve as a unique identifier for 
    * >  chemical structures. It contains separate blocks that define different 
    * >  layers of information, separated by dashes. For example, the full SPLASH 
    * >  of a caffeine mass spectrum above is splash10-0002-0900000000-b112e4e059e1ecf98c5f.
    * >  The first block is the SPLASH identifier, the second and third are 
    * >  summary blocks, and the fourth is the unique hash block.
    * >  
    * >  The SPLASH began As the MoNA (Massbank Of North America) hash, designed To 
    * >  identify duplicate spectra within the database. This idea developed further 
    * >  during the 2015 Metabolomics conference, where the SPLASH collaboration 
    * >  was formed. Currently, the specification has been formalized For mass 
    * >  spectrometry data. Additional specifications For IR, UV And NMR spectrometry
    * >  are planned.
    * 
     * @param spec The spectrum data, which can be a single spectrum object, a list, or an array of spectra.
     * @param type The type of spectrum (default is MS).
     * 
     * + default value Is ``null``.
     * @param env The R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A SPLASH ID string or an array/list of SPLASH IDs if input is multiple spectra.
   */
   function splash_id(spec: any, type?: object, env?: object): any;
   /**
    * Union and merge the given multiple spectrum data into one single spectrum
    * 
    * 
     * @param peaks A collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2`` spectrum object that going to merge into single one
     * @param norm 
     * + default value Is ``false``.
     * @param matrix this parameter will affects the data type of the value returns of this function:
     *  
     *  1. default false, returns a peak ms2 data object
     *  2. true, returns a library matrix data object
     * 
     * + default value Is ``false``.
     * @param massDiff the mass error for merge two spectra peak
     * 
     * + default value Is ``0.1``.
     * @param aggreate_sum default false means use the max intensity for the union merged peaks, 
     *  or use the sum value of the intensity if this parameter value is set as TRUE.
     * 
     * + default value Is ``false``.
     * @return a single ms spectrum data object, its data type depeneds on the **`matrix`** parameter.
   */
   function unionPeaks(peaks: object, norm?: boolean, matrix?: boolean, massDiff?: number, aggreate_sum?: boolean): object|object;
   /**
    * Extracts chromatogram data for a specific m/z from MS1 scans.
    * 
    * 
     * @param ms1 Input MS1 data (mzPack, PeakSet, or scan array).
     * @param mz Target m/z value to extract.
     * @param tolerance Mass tolerance for m/z matching.
     * 
     * + default value Is ``'ppm:20'``.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return ChromatogramTick array or ChromatogramOverlap object.
   */
   function XIC(ms1: any, mz: number, tolerance?: any, env?: object): object|object|object;
   /**
    * Groups MS1 scans into XIC (Extracted Ion Chromatogram) groups by m/z.
    * 
    * 
     * @param ms1 Input MS1 data (mzPack or array of scans).
     * @param tolerance Mass tolerance for grouping m/z values.
     * 
     * + default value Is ``'ppm:20'``.
     * @param env R environment for error handling.
     * 
     * + default value Is ``null``.
     * @return A list of XIC groups, each containing scans with similar m/z.
   */
   function XIC_groups(ms1: any, tolerance?: any, env?: object): any;
}
