// export R# package module type define for javascript/typescript language
//
//    imports "mzweb" from "mzkit";
//
// ref=mzkit.MzWeb@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mzweb {
   module as {
      /**
        * @param args default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function mzpack(assembly: any, args?: object, env?: object): object;
   }
   module load {
      /**
        * @param env default value Is ``null``.
      */
      function chromatogram(scans: any, env?: object): object;
      /**
        * @param mzErr default value Is ``'da:0.1'``.
        * @param env default value Is ``null``.
      */
      function stream(scans: object, mzErr?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function loadXcmsRData(file: any, env?: object): object;
   /**
     * @param mzdiff default value Is ``0.1``.
     * @param env default value Is ``null``.
   */
   function mass_calibration(data: object, mzdiff?: number, env?: object): object;
   /**
     * @param tolerance default value Is ``'da:0.001'``.
     * @param cutoff default value Is ``0.05``.
     * @param ionset default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function ms1_peaks(mzpack: object, tolerance?: any, cutoff?: number, ionset?: any, env?: object): object;
   /**
   */
   function ms1_scans(mzpack: object): object;
   /**
     * @param precursorMz default value Is ``NaN``.
     * @param tolerance default value Is ``'ppm:30'``.
     * @param tag_source default value Is ``true``.
     * @param centroid default value Is ``false``.
     * @param norm default value Is ``false``.
     * @param filter_empty default value Is ``true``.
     * @param into_cutoff default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function ms2_peaks(mzpack: object, precursorMz?: number, tolerance?: any, tag_source?: boolean, centroid?: boolean, norm?: boolean, filter_empty?: boolean, into_cutoff?: any, env?: object): object;
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function mzpack(file: any, env?: object): object;
   }
   module open_mzpack {
      /**
        * @param prefer default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function xml(file: string, prefer?: string, env?: object): object;
   }
   /**
   */
   function packBin(file: string, cache: string): ;
   module parse {
      /**
        * @param level default value Is ``[1,2]``.
        * @param env default value Is ``null``.
      */
      function scanMs(bytes: any, level?: any, env?: object): any;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function cache(file: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function setThumbnail(mzpack: object, thumb: any, env?: object): object;
   /**
   */
   function TIC(mzpack: object): object;
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function cache(ions: object, file: any, env?: object): boolean;
      /**
        * @param Ms2Only default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function cdf(mzpack: object, file: any, Ms2Only?: boolean, env?: object): any;
      /**
        * @param version default value Is ``2``.
        * @param headerSize default value Is ``-1``.
        * @param env default value Is ``null``.
      */
      function mzPack(mzpack: object, file: any, version?: object, headerSize?: object, env?: object): any;
      /**
        * @param file default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function text_cache(scans: object, file?: any, env?: object): any;
   }
}
