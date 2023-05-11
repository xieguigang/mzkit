// export R# package module type define for javascript/typescript language
//
// ref=mzkit.mzDeco@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mzDeco {
   module mz {
      /**
        * @param tolerance default value Is ``'ppm:20'``.
        * @param baseline default value Is ``0.65``.
        * @param peakwidth default value Is ``'3,20'``.
        * @param parallel default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function deco(ms1: any, tolerance?: any, baseline?: number, peakwidth?: any, parallel?: boolean, env?: object): object;
      /**
        * @param mzdiff default value Is ``'ppm:20'``.
        * @param env default value Is ``null``.
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
