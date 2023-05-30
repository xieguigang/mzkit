// export R# package module type define for javascript/typescript language
//
//    imports "GCMS" from "mz_quantify";
//
// ref=mzkit.GCMSLinear@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GCMS {
   module as {
      module quantify {
         /**
           * @param rtwin default value Is ``1``.
         */
         function ion(ions: object, rtwin?: number): object;
      }
   }
   /**
     * @param IS default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function contentTable(ions: any, contentVector: any, IS?: any, env?: object): object;
   /**
     * @param maxDeletions default value Is ``1``.
     * @param baselineQuantile default value Is ``0``.
     * @param integrator default value Is ``null``.
   */
   function linear_algorithm(contents: object, maxDeletions?: object, baselineQuantile?: number, integrator?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function linears(method: object, reference: any, env?: object): object;
   /**
   */
   function parseContents(files: any): object;
   /**
     * @param chromatogramPlot default value Is ``false``.
   */
   function peakRaw(extract: object, sample: object, chromatogramPlot?: boolean): object|object;
   module read {
      /**
      */
      function raw(file: string): object;
   }
   /**
     * @param peakwidth default value Is ``'3,20'``.
     * @param baseline default value Is ``0.65``.
     * @param sn default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function ROIlist(raw: object, peakwidth?: any, baseline?: number, sn?: number, env?: object): object;
   /**
     * @param peakwidth default value Is ``[3,5]``.
     * @param centroid default value Is ``'da:0.3'``.
     * @param rtshift default value Is ``30``.
     * @param baselineQuantile default value Is ``0.3``.
     * @param env default value Is ``null``.
   */
   function ScanIonExtractor(ions: object, peakwidth?: any, centroid?: any, rtshift?: number, baselineQuantile?: number, env?: object): object;
   /**
     * @param peakwidth default value Is ``[3,5]``.
     * @param centroid default value Is ``'da:0.3'``.
     * @param rtshift default value Is ``30``.
     * @param baselineQuantile default value Is ``0.3``.
     * @param env default value Is ``null``.
   */
   function SIMIonExtractor(ions: object, peakwidth?: any, centroid?: any, rtshift?: number, baselineQuantile?: number, env?: object): object;
}
