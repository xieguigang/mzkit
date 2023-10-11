// export R# package module type define for javascript/typescript language
//
//    imports "mzDeco" from "mz_quantify";
//
// ref=mzkit.mzDeco@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace mzDeco {
   module mz {
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
        * @param peakwidth 
        * + default value Is ``'3,20'``.
        * @param parallel 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function deco(ms1: any, tolerance?: any, baseline?: number, peakwidth?: any, parallel?: boolean, env?: object): object;
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
     * @param mzdiff default value Is ``'da:0.001'``.
     * @param rt_win default value Is ``30``.
     * @param norm default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function peak_alignment(samples: any, mzdiff?: any, rt_win?: number, norm?: boolean, env?: object): object;
   module read {
      /**
      */
      function peakFeatures(file: string): object;
   }
}
