﻿// export R# package module type define for javascript/typescript language
//
//    imports "mzDeco" from "mz_quantify";
//
// ref=mzkit.mzDeco@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Extract peak and signal data from rawdata
 *  
 *  Data processing is the computational process of converting raw LC-MS 
 *  data to biological knowledge and involves multiple processes including 
 *  raw data deconvolution and the chemical identification of metabolites.
 *  
 *  The process of data deconvolution, sometimes called peak picking, is 
 *  in itself a complex process caused by the complexity of the data and 
 *  variation introduced during the process of data acquisition related to 
 *  mass-to-charge ratio, retention time and chromatographic peak area.
 * 
*/
declare namespace mzDeco {
   /**
    * adjust the reteintion time data to unit seconds
    * 
    * 
     * @param rt_data -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function adjust_to_seconds(rt_data: any, env?: object): any;
   module as {
      /**
       * cast dataset to mzkit peaktable object
       * 
       * > for make data object conversion from a R# runtime dataframe object, that these data 
       * >  fields is required for creates the xcms peaks object:
       * >  
       * >  1. mz, mzmin, mzmax: the ion m/z value of the xcms peak
       * >  2. rt, rtmin, rtmax: the ion retention time of the xcms peak data, should be in time unit seconds
       * >  3. RI: the ion retention index value that evaluated based on the RT value
       * >  4. all of the other data fields in the dataframe will be treated as the sample peak area data.
       * 
        * @param x should be a data collection of the peaks data, value could be:
        *  
        *  1. a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` ROI peaks data
        *  2. an actual @``T:BioNovoGene.Analytical.MassSpectrometry.Math.PeakSet`` object, then this function will make value copy of this object
        *  3. a dataframe object that contains the peaks data for make the data conversion
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function peak_set(x: any, env?: object): object;
   }
   /**
    * make filter of the noise spectrum data
    * 
    * > this function will filter the noise spectrum data from the given 
    * >  msn level spectrum data collection
    * 
     * @param ions a collection of the msn level spectrum data
     * @param peaktable the peaktable object, is a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` object.
     * @param mzdiff the mass tolerance error in data unit delta dalton, 
     *  apply for matches between the peaktable precursor m/z and the given ion mz value.
     * 
     * + default value Is ``0.1``.
     * @param rt_win the rt window size for matches the rt. should be in data unit seconds.
     * 
     * + default value Is ``30``.
     * @param env 
     * + default value Is ``null``.
     * @return return a vector of clean spectrum object that could find any peak in ms1 table.
     *  additionally, the noise spectrum data will be set to the attribute named "noise" 
     *  of the return vector value.
     *  
     *  the return value is a vector of @``T:BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.PeakMs2`` object, and the noise
     *  spectrum data is set to the attribute named "noise" of the return value.
   */
   function filter_noise_spectrum(ions: any, peaktable: object, mzdiff?: number, rt_win?: number, env?: object): object;
   /**
    * helper function for find ms1 peaks based on the given mz/rt tuple data
    * 
    * 
     * @param peaktable the peaktable object, is a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` object.
     * @param mz target ion m/z
     * @param rt target ion rt in seconds.
     * @param mzdiff the mass tolerance error in data unit delta dalton, 
     *  apply for matches between the peaktable precursor m/z and the given ion mz value.
     * 
     * + default value Is ``0.01``.
     * @param rt_win the rt window size for matches the rt. should be in data unit seconds.
     * 
     * + default value Is ``90``.
     * @param find_RI 
     * + default value Is ``false``.
     * @return data is re-ordered via the tolerance error
   */
   function find_xcms_ionPeaks(peaktable: object, mz: number, rt: number, mzdiff?: number, rt_win?: number, find_RI?: boolean): object;
   /**
    * get ion peaks via the unique reference id
    * 
    * 
     * @param peaktable -
     * @param id a character vector of the unique reference id of the ion peaks
     * @param drop if the given id set contains a single id value, just returns the single xcms ion peak clr object,
     *  instead of a tuple list with single element? Default is not, which means this function 
     *  always returns the tuple list data by default.
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a tuple list of the xcms peaks object
   */
   function get_xcms_ionPeaks(peaktable: object, id: any, drop?: boolean, env?: object): object;
   /**
    * A debug function for test peak finding method
    * 
    * 
     * @param raw the file path of a single rawdata file.
     * @param massdiff -
     * 
     * + default value Is ``0.01``.
     * @param peak_width 
     * + default value Is ``[3,12]``.
     * @param q 
     * + default value Is ``0.65``.
     * @param sn_threshold 
     * + default value Is ``1``.
     * @param nticks 
     * + default value Is ``6``.
     * @param joint 
     * + default value Is ``true``.
   */
   function MS1deconv(raw: string, massdiff?: number, peak_width?: any, q?: number, sn_threshold?: number, nticks?: object, joint?: boolean): object;
   module mz {
      /**
       * do ``m/z`` grouping under the given tolerance
       * 
       * > the ion mz value is generated via the max intensity point in each ion 
       * >  feature group, and the xic data has already been re-order via the 
       * >  time asc.
       * 
        * @param ms1 a LCMS mzpack rawdata object or a collection of the ms1 point data
        * @param mzdiff the mass tolerance error for extract the XIC from the rawdata set
        * 
        * + default value Is ``'ppm:20'``.
        * @param rtwin the rt tolerance window size for merge data points
        * 
        * + default value Is ``0.05``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return create a list of XIC dataset for run downstream deconv operation
      */
      function groups(ms1: any, mzdiff?: any, rtwin?: number, env?: object): object;
   }
   /**
    * Chromatogram data deconvolution
    * 
    * 
     * @param ms1 a collection of the ms1 data or the mzpack raw data object, this parameter could also be
     *  a XIC pool object which contains a collection of the ion XIC data for run deconvolution.
     * @param tolerance the mass tolerance for extract the XIC data for run deconvolution.
     * 
     * + default value Is ``'ppm:20'``.
     * @param baseline 
     * + default value Is ``0.65``.
     * @param peak_width 
     * + default value Is ``'3,15'``.
     * @param joint 
     * + default value Is ``false``.
     * @param parallel run peak detection algorithm on mutliple xic data in parallel mode?
     * 
     * + default value Is ``false``.
     * @param dtw 
     * + default value Is ``false``.
     * @param feature a numeric vector of target feature ion m/z value for extract the XIC data.
     * 
     * + default value Is ``null``.
     * @param rawfile 
     * + default value Is ``null``.
     * @param sn_threshold 
     * + default value Is ``1``.
     * @param env 
     * + default value Is ``null``.
     * @return a vector of the peak deconvolution data,
     *  in format of xcms peak table liked or mzkit @``T:BioNovoGene.Analytical.MassSpectrometry.Math.PeakFeature``
     *  data object.
     *  
     *  the result data vector may contains the rt shift data result, where you can get this shift
     *  value via the ``rt.shift`` attribute name, rt shift data model is clr type: @``T:BioNovoGene.Analytical.MassSpectrometry.Math.RtShift``.
   */
   function mz_deco(ms1: any, tolerance?: any, baseline?: number, peak_width?: any, joint?: boolean, parallel?: boolean, dtw?: boolean, feature?: any, rawfile?: string, sn_threshold?: number, env?: object): object|object;
   /**
    * Do COW peak alignment and export peaktable
    *  
    *  Correlation optimized warping (COW) based on the total ion 
    *  current (TIC) is a widely used time alignment algorithm 
    *  (COW-TIC). This approach works successfully on chromatograms 
    *  containing few compounds and having a well-defined TIC.
    * 
    * 
     * @param samples should be a set of sample file data, which could be extract from the ``mz_deco`` function.
     * @param mzdiff -
     * 
     * + default value Is ``0.01``.
     * @param ri_win 
     * + default value Is ``10``.
     * @param norm do total ion sum normalization after peak alignment and the peaktable object has been exported?
     * 
     * + default value Is ``false``.
     * @param ri_alignment 
     * + default value Is ``false``.
     * @param max_intensity_ion 
     * + default value Is ``false``.
     * @param cow_alignment 
     * + default value Is ``false``.
     * @param aggregate 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peak_alignment(samples: any, mzdiff?: number, ri_win?: number, norm?: boolean, ri_alignment?: boolean, max_intensity_ion?: boolean, cow_alignment?: boolean, aggregate?: object, env?: object): object;
   /**
    * make sample column projection
    * 
    * 
     * @param peaktable A xcms liked peaktable object, is a collection 
     *  of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` peak feature data.
     * @param sampleNames A character vector of the sample names for make 
     *  the peaktable projection.
     * @param env 
     * + default value Is ``null``.
     * @return A sub-table of the input original peaktable data
   */
   function peak_subset(peaktable: object, sampleNames: any, env?: object): object;
   /**
    * extract a collection of xic data for a specific ion feature
    *  
    *  this function is debug used only
    * 
    * 
     * @param pool should be type of @``T:BioNovoGene.Analytical.MassSpectrometry.Math.XICPool`` or peak collection @``T:BioNovoGene.Analytical.MassSpectrometry.Math.PeakSet`` object.
     * @param mz the ion feature m/z value
     * @param dtw this parameter will not working when the data pool type is clr type @``T:BioNovoGene.Analytical.MassSpectrometry.Math.PeakSet``
     * 
     * + default value Is ``true``.
     * @param mzdiff -
     * 
     * + default value Is ``0.01``.
     * @param strict 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return a tuple list object that contains the xic data across
     *  multiple sample data files for a speicifc ion feature
     *  m/z.
   */
   function pull_xic(pool: any, mz: number, dtw?: boolean, mzdiff?: number, strict?: boolean, env?: object): any;
   module read {
      /**
       * read the peak feature table data
       * 
       * 
        * @param file -
        * @param readBin does the given data file is in binary format not a csv table file, 
        *  and this function should be parsed as a binary data file?
        * 
        * + default value Is ``false``.
      */
      function peakFeatures(file: string, readBin?: boolean): object;
      /**
      */
      function rt_shifts(file: string): object;
      /**
       * Try to cast the dataframe to the mzkit peak feature object set
       * 
       * 
        * @param file -
      */
      function xcms_features(file: object): object;
      /**
       * read the peaktable file that in xcms2 output format
       * 
       * 
        * @param file should be the file path to the peaktable csv/txt file.
        * @param tsv 
        * + default value Is ``false``.
        * @param general_method 
        * + default value Is ``false``.
        * @param make_unique set this parameter to value TRUE will ensure that the xcms reference id is always unique
        * 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
        * @return A collection set of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` peak features data object
      */
      function xcms_peaks(file: any, tsv?: boolean, general_method?: boolean, make_unique?: boolean, env?: object): object;
   }
   /**
    * make peaktable join of two batch data via (mz,RI)
    * 
    * 
     * @param batch1 -
     * @param batch2 -
     * @param mzdiff 
     * + default value Is ``0.01``.
     * @param ri_win 
     * + default value Is ``10``.
     * @param max_intensity_ion 
     * + default value Is ``false``.
     * @param aggregate 
     * + default value Is ``null``.
     * @return the ROI merge result across two sample batch data.
   */
   function RI_batch_join(batch1: object, batch2: object, mzdiff?: number, ri_win?: number, max_intensity_ion?: boolean, aggregate?: object): any;
   /**
    * RI calculation of a speicifc sample data
    * 
    * 
     * @param peakdata should be a collection of the peak data from a single sample file.
     * @param RI should be a collection of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.RIRefer`` data.
     * 
     * + default value Is ``null``.
     * @param ppm 
     * + default value Is ``20``.
     * @param dt 
     * + default value Is ``15``.
     * @param rawfile 
     * + default value Is ``null``.
     * @param by_id 
     * + default value Is ``false``.
     * @param C the number of carbon atoms for kovats retention index
     * 
     * + default value Is ``null``.
     * @param safe_wrap_missing 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function RI_cal(peakdata: object, RI?: any, ppm?: number, dt?: number, rawfile?: string, by_id?: boolean, C?: object, safe_wrap_missing?: boolean, env?: object): any;
   /**
    * Create RI reference dataset.
    * 
    * 
     * @param names 
     * + default value Is ``null``.
     * @param reference_mz 
     * + default value Is ``null``.
     * @param reference_rt 
     * + default value Is ``null``.
     * @return a collection of the mzkit ri reference object model 
     *  which is matched via the xcms peaktable.
   */
   function RI_reference(xcms_id: string, mz: number, rt: number, ri: number, names?: string, reference_mz?: number, reference_rt?: number): object;
   /**
    * Make peaks data group merge by rt directly
    * 
    * 
     * @param peaks -
     * @param dt -
     * 
     * + default value Is ``3``.
     * @param ppm -
     * 
     * + default value Is ``20``.
     * @param aggregate 
     * + default value Is ``null``.
   */
   function rt_groups(peaks: object, dt?: number, ppm?: number, aggregate?: object): object;
   /**
    * cast peaktable to expression matrix object
    * 
    * 
     * @param x -
   */
   function to_expression(x: object): object;
   /**
    * cast peaktable to mzkit expression matrix
    * 
    * 
     * @param x -
   */
   function to_matrix(x: object): object;
   module write {
      /**
       * write peak debug data
       * 
       * 
        * @param peaks -
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function peaks(peaks: any, file: any, env?: object): any;
      /**
       * save mzkit peaktable object to csv table file
       * 
       * 
        * @param x -
        * @param file the file path to the target csv table file
        * @param env 
        * + default value Is ``null``.
      */
      function xcms_peaks(x: object, file: any, env?: object): boolean;
   }
   /**
    * Create a xcms peak data object
    * 
    * 
     * @param id the unique referene id of the peak data
     * @param mz -
     * @param mz_range -
     * @param rt -
     * @param rt_range -
     * @param RI -
     * @param samples -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function xcms_peak(id: string, mz: number, mz_range: number, rt: number, rt_range: number, RI: number, samples: object, env?: object): object;
   /**
    * Load xic sample data files
    * 
    * 
     * @param files a character vector of a collection of the xic data files.
   */
   function xic_pool(files: string): object;
}
