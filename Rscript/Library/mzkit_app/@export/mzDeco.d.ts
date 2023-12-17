// export R# package module type define for javascript/typescript language
//
//    imports "mzDeco" from "mz_quantify";
//
// ref=mzkit.mzDeco@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Extract peak and signal data from rawdata
 * 
*/
declare namespace mzDeco {
   module mz {
      /**
       * do ``m/z`` grouping under the given tolerance
       * 
       * 
        * @param ms1 -
        * @param mzdiff -
        * 
        * + default value Is ``'ppm:20'``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return create a list of XIC dataset for run downstream deconv operation
      */
      function groups(ms1: any, mzdiff?: any, env?: object): object;
   }
   /**
    * Chromatogram data deconvolution
    * 
    * 
     * @param ms1 a collection of the ms1 data or the mzpack raw data object
     * @param tolerance -
     * 
     * + default value Is ``'ppm:20'``.
     * @param baseline 
     * + default value Is ``0.65``.
     * @param peak_width 
     * + default value Is ``'3,20'``.
     * @param joint 
     * + default value Is ``true``.
     * @param parallel 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return a vector of the peak deconvolution data
   */
   function mz_deco(ms1: any, tolerance?: any, baseline?: number, peak_width?: any, joint?: boolean, parallel?: boolean, env?: object): object;
   /**
    * Do COW peak alignment and export peaktable
    *  
    *  Correlation optimized warping (COW) based on the total ion 
    *  current (TIC) is a widely used time alignment algorithm 
    *  (COW-TIC). This approach works successfully on chromatograms 
    *  containing few compounds and having a well-defined TIC.
    * 
    * 
     * @param samples -
     * @param mzdiff -
     * 
     * + default value Is ``'da:0.001'``.
     * @param rt_win -
     * 
     * + default value Is ``30``.
     * @param norm -
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peak_alignment(samples: any, mzdiff?: any, rt_win?: number, norm?: boolean, env?: object): object;
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
}
