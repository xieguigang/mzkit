// export R# package module type define for javascript/typescript language
//
//    imports "mzDeco" from "mz_quantify";
//
// ref=mzkit.mzDeco@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mzDeco {
   module mz {
      /**
        * @param mzdiff default value Is ``'ppm:20'``.
        * @param rtwin default value Is ``0.05``.
        * @param env default value Is ``null``.
      */
      function groups(ms1: any, mzdiff?: any, rtwin?: number, env?: object): object;
   }
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param baseline default value Is ``0.65``.
     * @param peak_width default value Is ``'3,20'``.
     * @param joint default value Is ``false``.
     * @param parallel default value Is ``false``.
     * @param dtw default value Is ``false``.
     * @param feature default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function mz_deco(ms1: any, tolerance?: any, baseline?: number, peak_width?: any, joint?: boolean, parallel?: boolean, dtw?: boolean, feature?: any, env?: object): object|object;
   /**
     * @param mzdiff default value Is ``'da:0.001'``.
     * @param norm default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function peak_alignment(samples: any, mzdiff?: any, norm?: boolean, env?: object): object;
   /**
   */
   function peak_subset(peaktable: object, sampleNames: string): object;
   /**
     * @param dtw default value Is ``true``.
     * @param mzdiff default value Is ``0.01``.
   */
   function pull_xic(pool: object, mz: number, dtw?: boolean, mzdiff?: number): any;
   module read {
      /**
        * @param readBin default value Is ``false``.
      */
      function peakFeatures(file: string, readBin?: boolean): object;
      /**
      */
      function xcms_peaks(file: string): object;
   }
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function peaks(peaks: any, file: any, env?: object): any;
   }
   /**
   */
   function xic_pool(files: string): object;
}
