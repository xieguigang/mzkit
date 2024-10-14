// export R# package module type define for javascript/typescript language
//
//    imports "Linears" from "mz_quantify";
//
// ref=mzkit.Linears@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * targeted linears
 * 
*/
declare namespace Linears {
   /**
    * get ions peaks
    * 
    * 
     * @param samples -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function ionPeaks(samples: any, env?: object): object;
   module lines {
      /**
       * export the table data of the reference linears
       * 
       * 
        * @param lines -
      */
      function table(lines: object): object;
   }
   /**
    * Get reference input points
    * 
    * 
     * @param linears -
     * @param nameRef The metabolite id
     * @param env 
     * + default value Is ``null``.
   */
   function points(linears: object, nameRef: any, env?: object): object;
   /**
    * Run sample data quantify
    * 
    * 
     * @param models -
     * @param ions -
     * @param integrator 
     * + default value Is ``null``.
     * @param names 
     * + default value Is ``null``.
     * @param baselineQuantile 
     * + default value Is ``0.6``.
     * @param fileName 
     * + default value Is ``'NA'``.
     * @param env 
     * + default value Is ``null``.
   */
   function quantify(models: object, ions: object, integrator?: object, names?: object, baselineQuantile?: number, fileName?: string, env?: object): object;
   module read {
      /**
      */
      function linearPack(file: string): object;
   }
   module report {
      /**
       * Create targeted linear dataset object for do linear quantification data report.
       * 
       * 
        * @param standardCurve -
        * @param samples -
        * @param QC_dataset Regular expression pattern string for match QC sample files
        * 
        * + default value Is ``null``.
        * @param ionsRaw A list of ions ChromatogramTick data for run ion data plots.
        *  the list data structure in format looks like:
        *  
        *  ```
        *  { 
        *     ion_id1 {
        *         sample1: ChromatogramTick[],
        *         sample2: ChromatogramTick[],
        *         ...
        *     }
        *  }
        *  ```
        * 
        * + default value Is ``null``.
      */
      function dataset(standardCurve: object, samples: object, QC_dataset?: string, ionsRaw?: object): object|object;
   }
   /**
    * Get quantify result
    * 
    * 
     * @param fileScans -
   */
   function result(fileScans: object): object;
   module scans {
      /**
       * Get result of ``AIS/At``
       * 
       * 
        * @param fileScans -
      */
      function X(fileScans: object): object;
   }
   module write {
      /**
       * Write peak data which is extract from the raw file with 
       *  the given ion pairs or quantify ion data
       * 
       * 
        * @param ionPeaks -
        * @param file The output csv file path
      */
      function ionPeaks(ionPeaks: object, file: string): boolean;
      /**
       * save reference point data into a given table file
       * 
       * 
        * @param points -
      */
      function points(points: object, file: string): boolean;
   }
}
