// export R# package module type define for javascript/typescript language
//
//    imports "math" from "mzkit";
//    imports "math" from "mz_quantify";
//
// ref=mzkit.MzMath@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=mzkit.QuantifyMath@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace math {
   /**
     * @param tolerance default value Is ``'da:0.1'``.
     * @param intoCutoff default value Is ``0.05``.
     * @param parallel default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function centroid(ions: any, tolerance?: any, intoCutoff?: number, parallel?: boolean, env?: object): object|object|number;
   /**
     * @param x default value Is ``null``.
     * @param time default value Is ``'Time'``.
     * @param into default value Is ``'Intensity'``.
     * @param env default value Is ``null``.
   */
   function chromatogram(x?: any, time?: any, into?: any, env?: object): object;
   module cluster {
      /**
      */
      function nodes(tree: object): object;
   }
   module cosine {
      /**
        * @param tolerance default value Is ``'da:0.3'``.
        * @param intocutoff default value Is ``0.05``.
        * @param env default value Is ``null``.
      */
      function pairwise(query: any, ref: any, tolerance?: any, intocutoff?: number, env?: object): any;
   }
   /**
   */
   function defaultPrecursors(ionMode: string): object;
   /**
     * @param mode default value Is ``'+'``.
   */
   function exact_mass(mz: number, mode?: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function intensity_vec(ms: any, mzSet: object, env?: object): number;
   module ions {
      /**
        * @param eq default value Is ``0.85``.
        * @param gt default value Is ``0.6``.
        * @param mzwidth default value Is ``'da:0.1'``.
        * @param tolerance default value Is ``'da:0.3'``.
        * @param precursor default value Is ``'ppm:20'``.
        * @param rtwidth default value Is ``5``.
        * @param trim default value Is ``'0.05'``.
        * @param env default value Is ``null``.
      */
      function unique(ions: any, eq?: number, gt?: number, mzwidth?: string, tolerance?: string, precursor?: string, rtwidth?: number, trim?: string, env?: object): any;
   }
   /**
     * @param tolerance default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function jaccard(query: number, ref: number, tolerance?: any, env?: object): any;
   /**
     * @param tolerance default value Is ``'da:0.3'``.
     * @param env default value Is ``null``.
   */
   function jaccardSet(query: number, ref: number, tolerance?: any, env?: object): any;
   /**
     * @param mode default value Is ``'+'``.
     * @param env default value Is ``null``.
   */
   function mz(mass: number, mode?: any, env?: object): object|number;
   /**
   */
   function mz_index(mz: number): object;
   /**
     * @param sum default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function norm_msdata(msdata: any, sum?: boolean, env?: object): object;
   /**
     * @param baselineQuantile default value Is ``0.65``.
     * @param angleThreshold default value Is ``5``.
     * @param peakwidth default value Is ``'8,30'``.
     * @param sn_threshold default value Is ``3``.
     * @param joint default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function peakROI(chromatogram: any, baselineQuantile?: number, angleThreshold?: number, peakwidth?: any, sn_threshold?: number, joint?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function ppm(a: any, b: any, env?: object): number;
   /**
     * @param env default value Is ``null``.
   */
   function precursor_types(types: any, env?: object): any;
   /**
     * @param dt default value Is ``1``.
     * @param aggregate default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function resample(TIC: object, dt?: number, aggregate?: object, env?: object): any;
   /**
     * @param mzwidth default value Is ``'da:0.1'``.
     * @param rtwidth default value Is ``60``.
     * @param env default value Is ``null``.
   */
   function sequenceOrder(scans: any, mzwidth?: any, rtwidth?: number, env?: object): any;
   /**
     * @param ref default value Is ``null``.
     * @param tolerance default value Is ``'da:0.3'``.
     * @param intocutoff default value Is ``0.05``.
     * @param env default value Is ``null``.
   */
   function spectral_entropy(x: object, ref?: object, tolerance?: any, intocutoff?: number, env?: object): any;
   module spectrum {
      /**
        * @param tolerance default value Is ``'da:0.1'``.
        * @param equals_score default value Is ``0.85``.
        * @param gt_score default value Is ``0.6``.
        * @param score_aggregate default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function compares(tolerance?: any, equals_score?: number, gt_score?: number, score_aggregate?: object, env?: object): object;
   }
   module spectrum_tree {
      /**
        * @param compares default value Is ``null``.
        * @param tolerance default value Is ``'da:0.1'``.
        * @param intocutoff default value Is ``0.05``.
        * @param showReport default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function cluster(ms2list: any, compares?: object, tolerance?: any, intocutoff?: number, showReport?: boolean, env?: object): object;
   }
   /**
     * @param method default value Is ``["ppm","da"]``.
     * @param env default value Is ``null``.
   */
   function tolerance(threshold: number, method?: any, env?: object): any;
   /**
   */
   function toMS(isotope: object): object;
   /**
   */
   function xcms_id(mz: number, rt: number): string;
}
