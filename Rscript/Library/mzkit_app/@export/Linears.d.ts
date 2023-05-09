// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Linears@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace Linears {
   /**
     * @param env default value Is ``null``.
   */
   function ionPeaks(samples:any, env?:object): object;
   module lines {
      /**
      */
      function table(lines:object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function points(linears:object, nameRef:any, env?:object): object;
   /**
     * @param integrator default value Is ``null``.
     * @param names default value Is ``null``.
     * @param baselineQuantile default value Is ``0.6``.
     * @param fileName default value Is ``'NA'``.
     * @param env default value Is ``null``.
   */
   function quantify(models:object, ions:object, integrator?:object, names?:object, baselineQuantile?:number, fileName?:string, env?:object): object;
   module report {
      /**
        * @param QC_dataset default value Is ``null``.
        * @param ionsRaw default value Is ``null``.
      */
      function dataset(standardCurve:object, samples:object, QC_dataset?:string, ionsRaw?:object): any;
   }
   /**
   */
   function result(fileScans:object): object;
   module scans {
      /**
      */
      function X(fileScans:object): object;
   }
   module write {
      /**
      */
      function ionPeaks(ionPeaks:object, file:string): boolean;
      /**
      */
      function points(points:object, file:string): boolean;
   }
}
