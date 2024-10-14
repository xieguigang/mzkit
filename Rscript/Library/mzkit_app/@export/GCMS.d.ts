// export R# package module type define for javascript/typescript language
//
//    imports "GCMS" from "mz_quantify";
//
// ref=mzkit.GCMSLinear@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the targetted GCMS sim data quantification module
 * 
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
    * create linear model handler
    * 
    * 
     * @param contents -
     * @param maxDeletions -
     * 
     * + default value Is ``1``.
     * @param baselineQuantile 
     * + default value Is ``0``.
     * @param integrator 
     * + default value Is ``null``.
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
    * read raw peaks data
    * 
    * 
     * @param extract -
     * @param sample -
     * @param chromatogramPlot 
     * + default value Is ``false``.
   */
   function peakRaw(extract: object, sample: object, chromatogramPlot?: boolean): object|object;
   module read {
      /**
       * read gcms rawdata
       * 
       * 
        * @param file -
      */
      function raw(file: string): object;
   }
   /**
    * do peak detection for gc-ms rawdata
    * 
    * 
     * @param raw the input gcms rawdata, could be @``T:BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.Raw`` for 
     *  targetted data and @``T:BioNovoGene.Analytical.MassSpectrometry.Assembly.mzPack`` for un-targetted gc-ms rawdata.
     * @param peakwidth -
     * 
     * + default value Is ``'3,20'``.
     * @param baseline -
     * 
     * + default value Is ``0.65``.
     * @param sn -
     * 
     * + default value Is ``3``.
     * @param joint 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a list of detected peak features
   */
   function ROIlist(raw: any, peakwidth?: any, baseline?: number, sn?: number, joint?: boolean, env?: object): object|object;
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
