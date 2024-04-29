// export R# package module type define for javascript/typescript language
//
//    imports "math" from "mz_quantify";
//
// ref=mzkit.QuantifyMath@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace math {
   /**
    * Create a chromatogram data from a dataframe object
    * 
    * 
     * @param x Should be a dataframe object that contains 
     *  the required data field for construct the chromatogram data.
     * 
     * + default value Is ``null``.
     * @param time the column name for get the rt field vector data
     * 
     * + default value Is ``'Time'``.
     * @param into the column name for get the signal intensity field vector data
     * 
     * + default value Is ``'Intensity'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function chromatogram(x?: any, time?: any, into?: any, env?: object): object;
   /**
    * ### Peak finding
    *  
    *  Extract the peak ROI data from the chromatogram data
    * 
    * 
     * @param chromatogram -
     * @param baselineQuantile -
     * 
     * + default value Is ``0.65``.
     * @param angleThreshold -
     * 
     * + default value Is ``5``.
     * @param peakwidth -
     * 
     * + default value Is ``'8,30'``.
     * @param sn_threshold -
     * 
     * + default value Is ``3``.
     * @param joint 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function peakROI(chromatogram: any, baselineQuantile?: number, angleThreshold?: number, peakwidth?: any, sn_threshold?: number, joint?: boolean, env?: object): object;
   /**
    * Do resample of the chromatogram data
    * 
    * 
     * @param TIC -
     * @param dt -
     * 
     * + default value Is ``1``.
     * @param aggregate 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function resample(TIC: object, dt?: number, aggregate?: any, env?: object): any;
}
