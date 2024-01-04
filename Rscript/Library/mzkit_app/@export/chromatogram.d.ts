// export R# package module type define for javascript/typescript language
//
//    imports "chromatogram" from "mzkit";
//
// ref=mzkit.ChromatogramTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace chromatogram {
   /**
   */
   function add(overlaps: object, name: string, data: object): object;
   module as {
      /**
        * @param args default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function chromatogram(scans: any, args?: object, env?: object): object|object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function labels(overlaps: object, names: string, env?: object): object;
   /**
     * @param TIC default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function overlaps(TIC?: any, env?: object): object;
   /**
     * @param dt default value Is ``0.3``.
   */
   function overlapsMatrix(overlaps: object, dt?: number): any;
   module read {
      /**
      */
      function pack(cdf: string): object;
   }
   /**
     * @param unit default value Is ``'minute'``.
   */
   function scale_time(overlaps: object, unit?: string): object;
   /**
   */
   function subset(overlaps: object, names: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function toChromatogram(ticks: any, env?: object): object;
   /**
   */
   function topInto(overlaps: object, n: object): object;
   module write {
      /**
      */
      function pack(overlaps: object, cdf: string): ;
   }
}
