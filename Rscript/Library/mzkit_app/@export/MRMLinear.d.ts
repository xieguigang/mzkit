// export R# package module type define for javascript/typescript language
//
//    imports "MRMLinear" from "mz_quantify";
//
// ref=mzkit.MRMkit@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace MRMLinear {
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function ion_pairs(mz: any, env?: object): object;
   }
   module extract {
      /**
        * @param tolerance default value Is ``'ppm:20'``.
        * @param env default value Is ``null``.
      */
      function ions(mzML: string, ionpairs: object, tolerance?: any, env?: object): object;
      /**
        * @param tolerance default value Is ``'ppm:20'``.
        * @param timeWindowSize default value Is ``5``.
        * @param baselineQuantile default value Is ``0.65``.
        * @param integratorTicks default value Is ``5000``.
        * @param peakAreaMethod default value Is ``null``.
        * @param angleThreshold default value Is ``5``.
        * @param peakwidth default value Is ``'8,30'``.
        * @param rtshift default value Is ``null``.
        * @param bsplineDensity default value Is ``100``.
        * @param bsplineDegree default value Is ``2``.
        * @param sn_threshold default value Is ``3``.
        * @param TPAFactors default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function peakROI(mzML: string, ionpairs: object, tolerance?: string, timeWindowSize?: number, baselineQuantile?: number, integratorTicks?: object, peakAreaMethod?: object, angleThreshold?: number, peakwidth?: any, rtshift?: object, bsplineDensity?: object, bsplineDegree?: object, sn_threshold?: number, TPAFactors?: object, env?: object): object;
   }
   module isomerism {
      /**
        * @param tolerance default value Is ``'ppm:20'``.
        * @param env default value Is ``null``.
      */
      function ion_pairs(ions: object, tolerance?: string, env?: object): object;
   }
   /**
     * @param blankControls default value Is ``null``.
     * @param maxDeletions default value Is ``1``.
     * @param isWorkCurveMode default value Is ``true``.
     * @param args default value Is ``null``.
   */
   function linears(rawScan: object, calibrates: object, ISvector: object, blankControls?: object, maxDeletions?: object, isWorkCurveMode?: boolean, args?: object): object;
   module MRM {
      /**
        * @param tolerance default value Is ``'da:0.3'``.
        * @param timeWindowSize default value Is ``5``.
        * @param angleThreshold default value Is ``5``.
        * @param baselineQuantile default value Is ``0.65``.
        * @param integratorTicks default value Is ``5000``.
        * @param peakAreaMethod default value Is ``null``.
        * @param peakwidth default value Is ``'8,30'``.
        * @param TPAFactors default value Is ``null``.
        * @param sn_threshold default value Is ``3``.
        * @param env default value Is ``null``.
      */
      function arguments(tolerance?: any, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, integratorTicks?: object, peakAreaMethod?: object, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, env?: object): object;
      /**
        * @param rtshifts default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function peak2(mzML: string, ions: object, args: object, rtshifts?: object, env?: object): object;
      /**
        * @param peakAreaMethod default value Is ``null``.
        * @param tolerance default value Is ``'ppm:20'``.
        * @param timeWindowSize default value Is ``5``.
        * @param angleThreshold default value Is ``5``.
        * @param baselineQuantile default value Is ``0.65``.
        * @param rtshifts default value Is ``null``.
        * @param TPAFactors default value Is ``null``.
        * @param peakwidth default value Is ``'8,30'``.
        * @param sn_threshold default value Is ``3``.
        * @param env default value Is ``null``.
      */
      function peaks(mzML: string, ions: object, peakAreaMethod?: object, tolerance?: string, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, rtshifts?: object, TPAFactors?: object, peakwidth?: any, sn_threshold?: number, env?: object): object;
      /**
        * @param args default value Is ``null``.
      */
      function rt_alignments(cal: any, ions: any, args?: object): object;
   }
   /**
     * @param baselineQuantile default value Is ``0.65``.
     * @param angleThreshold default value Is ``5``.
     * @param peakwidth default value Is ``'8,30'``.
     * @param sn_threshold default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function peakROI(chromatogram: any, baselineQuantile?: number, angleThreshold?: number, peakwidth?: any, sn_threshold?: number, env?: object): object;
   /**
   */
   function R2(lines: object): number;
   module read {
      /**
        * @param sheetName default value Is ``'Sheet1'``.
      */
      function ion_pairs(file: string, sheetName?: string): object;
      /**
        * @param sheetName default value Is ``'Sheet1'``.
        * @param env default value Is ``null``.
      */
      function IS(file: string, sheetName?: string, env?: object): object;
      /**
        * @param sheetName default value Is ``'Sheet1'``.
      */
      function reference(file: string, sheetName?: string): object;
   }
   module sample {
      /**
        * @param peakAreaMethod default value Is ``null``.
        * @param tolerance default value Is ``'ppm:20'``.
        * @param timeWindowSize default value Is ``5``.
        * @param angleThreshold default value Is ``5``.
        * @param baselineQuantile default value Is ``0.65``.
        * @param peakwidth default value Is ``'8,30'``.
        * @param TPAFactors default value Is ``null``.
        * @param sn_threshold default value Is ``3``.
        * @param env default value Is ``null``.
      */
      function quantify(model: object, file: string, ions: object, peakAreaMethod?: object, tolerance?: string, timeWindowSize?: number, angleThreshold?: number, baselineQuantile?: number, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function quantify2(model: object, file: string, ions: object, env?: object): object;
   }
   module wiff {
      /**
        * @param patternOfRef default value Is ``'.+[-]CAL[-]?\d+'``.
        * @param patternOfBlank default value Is ``'KB[-]?(\d+)?'``.
        * @param env default value Is ``null``.
      */
      function rawfiles(convertDir: any, patternOfRef?: string, patternOfBlank?: string, env?: object): any;
      /**
        * @param args default value Is ``null``.
        * @param rtshifts default value Is ``null``.
        * @param removesWiffName default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function scan2(wiffConverts: string, ions: object, args?: object, rtshifts?: object, removesWiffName?: boolean, env?: object): object;
      /**
        * @param peakAreaMethod default value Is ``null``.
        * @param tolerance default value Is ``'ppm:20'``.
        * @param angleThreshold default value Is ``5``.
        * @param baselineQuantile default value Is ``0.65``.
        * @param removesWiffName default value Is ``true``.
        * @param timeWindowSize default value Is ``5``.
        * @param rtshifts default value Is ``null``.
        * @param bsplineDensity default value Is ``100``.
        * @param bsplineDegree default value Is ``2``.
        * @param resolution default value Is ``3000``.
        * @param peakwidth default value Is ``'8,30'``.
        * @param TPAFactors default value Is ``null``.
        * @param sn_threshold default value Is ``3``.
        * @param env default value Is ``null``.
      */
      function scans(wiffConverts: string, ions: object, peakAreaMethod?: object, tolerance?: string, angleThreshold?: number, baselineQuantile?: number, removesWiffName?: boolean, timeWindowSize?: number, rtshifts?: object, bsplineDensity?: object, bsplineDegree?: object, resolution?: object, peakwidth?: any, TPAFactors?: object, sn_threshold?: number, env?: object): object;
   }
}
