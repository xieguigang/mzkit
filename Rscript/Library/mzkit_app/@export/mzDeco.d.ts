// export R# package module type define for javascript/typescript language
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
     * @param mzdiff default value Is ``0.01``.
     * @param rt_win default value Is ``90``.
   */
   function find_xcms_ionPeaks(peaktable: object, mz: number, rt: number, mzdiff?: number, rt_win?: number): object;
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
     * + default value Is ``'3,20'``.
     * @param joint 
     * + default value Is ``false``.
     * @param parallel 
     * + default value Is ``false``.
     * @param dtw 
     * + default value Is ``false``.
     * @param feature a numeric vector of target feature ion m/z value for extract the XIC data.
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return a vector of the peak deconvolution data,
     *  in format of xcms peak table liked or mzkit @``T:BioNovoGene.Analytical.MassSpectrometry.Math.PeakFeature``
     *  data object.
     *  
     *  the result data vector may contains the rt shift data result, where you can get this shift
     *  value via the ``rt.shift`` attribute name, rt shift data model is clr type: @``T:BioNovoGene.Analytical.MassSpectrometry.Math.RtShift``.
   */
   function mz_deco(ms1: any, tolerance?: any, baseline?: number, peak_width?: any, joint?: boolean, parallel?: boolean, dtw?: boolean, feature?: any, env?: object): object|object;
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
     * + default value Is ``'da:0.001'``.
     * @param norm do total ion sum normalization after peak alignment and the peaktable object has been exported?
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peak_alignment(samples: any, mzdiff?: any, norm?: boolean, env?: object): object;
   /**
   */
   function peak_subset(peaktable: object, sampleNames: string): object;
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
       * read the peaktable file that in xcms2 output format
       * 
       * 
        * @param file -
        * @param tsv 
        * + default value Is ``false``.
        * @param general_method 
        * + default value Is ``false``.
        * @return A collection set of the @``T:BioNovoGene.Analytical.MassSpectrometry.Math.xcms2`` peak features data object
      */
      function xcms_peaks(file: string, tsv?: boolean, general_method?: boolean): object;
   }
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
   }
   /**
   */
   function xic_pool(files: string): object;
}
